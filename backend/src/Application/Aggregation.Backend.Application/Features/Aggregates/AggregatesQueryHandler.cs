using Aggregation.Backend.Application.Interfaces;
using MediatR;

namespace Aggregation.Backend.Application.Features.Aggregates
{
    public class AggregatesQueryHandler(IAirPollutionService airPollutionService,INewsService newsService,IStockMarketFeedService stockMarketFeedService) : IRequestHandler<AggregatesQuery, IList<Dictionary<string, object>>>
    {
        public async Task<IList<Dictionary<string, object>>> Handle(AggregatesQuery request, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IList<Dictionary<string, object>>>> {
                airPollutionService.ListAsync(request.Category, cancellationToken),
                newsService.ListAsync(request.Category, cancellationToken),
                stockMarketFeedService.ListAsync(request.Category, cancellationToken) 
            };
            

            var taskResults = await Task.WhenAll(tasks);

            var result = taskResults.SelectMany(r => r).ToList();

            if (!string.IsNullOrEmpty(request.FilterBy))
            {
                var filter = request.FilterBy.Split('=');
                result = result.Where(s => s.TryGetValue(filter[0], out var value) && value == filter[1]).ToList();
            }

            if (!string.IsNullOrEmpty(request.SortBy) && result.All(s => s.TryGetValue(request.SortBy, out _)))
            {
                result = request.Asc ?
                    result.OrderBy(s => s[request.SortBy]).ToList()
                    : result.OrderByDescending(s => s[request.SortBy]).ToList();
            }

            return result;
        }
    }
}