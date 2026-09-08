using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Domain.Dtos.External.Air;
using Aggregation.Backend.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aggregation.Backend.Infrastructure.Services
{
    public class AirPollutionService(IHttpClientWrapper<AirPollutionOptions> httpClientWrapper, IOptions<AirPollutionOptions> options) : IAirPollutionService
    {

        public async Task<IList<Dictionary<string, object>>> ListAsync(string category, CancellationToken cancellationToken)
        {
            var queryString = $"state={category}&country=Greece&key={options.Value.ApiKey}";
            var relativePath = string.Join("?", options.Value.ListUri, queryString);
            var response = await httpClientWrapper.GetAsync(relativePath, cancellationToken);

            if (string.IsNullOrEmpty(response))
            {
                return [];
            }

            var cities = JsonConvert.DeserializeObject<CitiesDto>(response);

            var tasks = new List<Task<Dictionary<string, object>>>();
            foreach (var item in cities?.Cities ?? Enumerable.Empty<NameOnlyDto>())
            {
                var cityName = item.City;
                tasks.Add(Task.Run(async () =>
                {
                    var queryStringPerCity = $"city={cityName}&state={category}&country=Greece&key={options.Value.ApiKey}";
                    var relativePathPerCity = string.Join("?", options.Value.GetUri, queryStringPerCity);

                    var cityResponse = await httpClientWrapper.GetAsync(relativePathPerCity, cancellationToken);
                    if (string.IsNullOrEmpty(cityResponse))
                    {
                        return new Dictionary<string, object>();
                    }
                    var city = JsonConvert.DeserializeObject<CityDto>(cityResponse);
                    var result = city?.Data?.ToMap() ?? new Dictionary<string, object>();
                    return result;
                }));
            }

            var results = await Task.WhenAll(tasks);
            return results.Where(s => s.Count > 0).ToList();
        }
    }
}