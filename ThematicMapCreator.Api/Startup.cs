using System;
using BAMCIS.GeoJSON;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ThematicMapCreator.Api.Contracts;
using ThematicMapCreator.Api.Contracts.LayerStyleOptions;
using ThematicMapCreator.Api.Core;
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

            ConfigureMapping();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new KeyJsonConverter<ILayerStyleOptions, LayerStyle>(styleOptions => styleOptions.Style)
                    .RegisterType<SimpleStyleOptions>(LayerStyle.None)
                    .RegisterType<UniqueValuesStyleOptions>(LayerStyle.UniqueValues));
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

            services.AddDbContextDesignTimeServices();
        }

        private static void ConfigureMapping()
        {
            // TODO убрать повторение создания конвертера.
            JsonConverter converter = new KeyJsonConverter<ILayerStyleOptions, LayerStyle>(styleOptions => styleOptions.Style)
                .RegisterType<SimpleStyleOptions>(LayerStyle.None)
                .RegisterType<UniqueValuesStyleOptions>(LayerStyle.UniqueValues);

            TypeAdapterConfig<Layer, LayerOverview>.NewConfig()
                .Map(dest => dest.Data, source => JsonConvert.DeserializeObject<GeoJson>(source.Data))
                .Map(dest => dest.StyleOptions, source => JsonConvert.DeserializeObject<ILayerStyleOptions>(source.StyleOptions, converter));
            TypeAdapterConfig<LayerOverview, Layer>.NewConfig()
                .Map(dest => dest.Data, source => JsonConvert.SerializeObject(source.Data))
                .Map(dest => dest.StyleOptions, source => JsonConvert.SerializeObject(source.StyleOptions));

            TypeAdapterConfig<SaveMapRequest, Map>.NewConfig()
                .Map(dest => dest.Layers, source => null as object);
        }
    }
}
