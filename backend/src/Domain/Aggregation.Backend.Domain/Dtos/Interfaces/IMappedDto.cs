namespace Aggregation.Backend.Domain.Dtos.Interfaces
{
    public interface IMappedDto
    {
        Dictionary<string, object> ToMap();
    }
}