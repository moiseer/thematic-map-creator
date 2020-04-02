using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ThematicMapCreator.Api.Migrations;
using ThematicMapCreator.Api.Models;

namespace ThematicMapCreator.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Map"));

            app.UseDatabaseMigration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(options => options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "Thematic Map Creator"
                })
            );

            services.AddDbContext<ThematicMapDbContext>(options => options
                .UseSqlServer(Configuration.GetConnectionString("ThematicMapDb"))
            );

            services.AddDbContextDesignTimeServices();
        }
    }
}
