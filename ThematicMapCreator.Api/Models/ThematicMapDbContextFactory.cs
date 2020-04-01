using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ThematicMapCreator.Api.Models
{
    public class ThematicMapDbContextFactory : IDesignTimeDbContextFactory<ThematicMapDbContext>
    {
        private readonly IConfiguration configuration;

        public ThematicMapDbContextFactory()
        {
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        public ThematicMapDbContext CreateDbContext(string[] args)
        {
            DbContextOptions<ThematicMapDbContext> options = new DbContextOptionsBuilder<ThematicMapDbContext>()
                .UseSqlServer(configuration.GetConnectionString("ThematicMapDb"))
                .Options;

            return new ThematicMapDbContext(options);
        }
    }
}
