using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ThematicMapCreator.Api.Contracts.LayerStyleOptions;
using ThematicMapCreator.Api.Core;
using ThematicMapCreator.Api.Migrations;
using ThematicMapCreator.Api.Models;
using ThematicMapCreator.Api.Services;

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
            app.UseMapping();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var layerStyleOptionsConverter = new KeyJsonConverter<ILayerStyleOptions, LayerStyle>(styleOptions => styleOptions.Style)
                .RegisterType<SimpleStyleOptions>(LayerStyle.None)
                .RegisterType<UniqueValuesStyleOptions>(LayerStyle.UniqueValues)
                .RegisterType<GraduatedColorsStyleOptions>(LayerStyle.GraduatedColors);
            services.AddSingleton(layerStyleOptionsConverter);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(layerStyleOptionsConverter);
            });

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

            services.AddSingleton<FilesService>();
        }
    }
}
