
using Aggregation.Backend.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aggregation.Backend.Infrastructure.Data.Contexts;

public class AggregationBackendIdentityDbContext : IdentityDbContext<AggregationBackendUser>
{
    public AggregationBackendIdentityDbContext(DbContextOptions<AggregationBackendIdentityDbContext> options)
        : base(options)
    {
    }

}
