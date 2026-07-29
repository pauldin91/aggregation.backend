using Aggregation.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aggregation.Backend.Infrastructure.Data.Configurations
{
    public class AggregationBackendUserEntityTypeConfiguration : IEntityTypeConfiguration<AggregationBackendUser>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AggregationBackendUser> builder)
        {
            throw new NotImplementedException();
        }
    }
}