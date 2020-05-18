using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using ThematicMapCreator.Api.Models;

namespace ThematicMapCreator.Api.Migrations
{
    public static class DatabaseMigrationExtensions
    {
        public static IServiceCollection AddDbContextDesignTimeServices(this IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ThematicMapDbContext>();
            services.AddDbContextDesignTimeServices(context);

            return services;
        }

        public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ThematicMapDbContext>();
            context.Database.Migrate();

            return app;
        }
    }
}
