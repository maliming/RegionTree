using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RegionTree
{
    public class RegionDbContext : AbpDbContext
    {
        public RegionDbContext(DbContextOptions<RegionDbContext> options) : base(options)
        {
        }

        public DbSet<Region> Region { get; set; }
    }

    /// <summary>
    ///     dotnet ef ....
    /// </summary>
    public class RegionDbContextFactory : IDesignTimeDbContextFactory<RegionDbContext>
    {
        public RegionDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RegionDbContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=Region;Trusted_Connection=True;");

            return new RegionDbContext(optionsBuilder.Options);
        }
    }
}