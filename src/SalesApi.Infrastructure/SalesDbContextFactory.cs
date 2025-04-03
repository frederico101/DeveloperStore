using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Structure;
using System.IO;

namespace SalesApi.Infrastructure
{
    public class SalesDbContextFactory : IDesignTimeDbContextFactory<SalesDbContext>
    {
        public SalesDbContext CreateDbContext(string[] args)
        {
            //var basePath = Directory.GetCurrentDirectory();
            //var configurationBuilder = new ConfigurationBuilder()
            //    .SetBasePath(basePath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            //var configuration = configurationBuilder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<SalesDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=SalesApiDb;User Id=sa;Password=FredStrongPa55; TrustServerCertificate=True");

            return new SalesDbContext(optionsBuilder.Options);
        }
    }
}