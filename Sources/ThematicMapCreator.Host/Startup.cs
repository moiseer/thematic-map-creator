using Core.Dal.EntityFramework;
using Core.Dal.EntityFramework.Extensions;
using Core.Dal.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using ThematicMapCreator.Contracts;
using ThematicMapCreator.Domain;
using ThematicMapCreator.Domain.Repositories;
using ThematicMapCreator.Domain.Services;
using ThematicMapCreator.Domain.Validators;
using ThematicMapCreator.Host.Filters;
using ThematicMapCreator.Host.Persistence.Contexts;
using ThematicMapCreator.Host.Persistence.Repositories;

namespace ThematicMapCreator.Host
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

            app.UseSerilogRequestLogging();

            app.UseCors(builder => builder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Thematic Map Creator"));

            UseMigration(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.AddService<TmcExceptionFilter>());

            services.AddSwaggerGen(options => options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "Thematic Map Creator"
                }));

            services.AddSingleton<TmcExceptionFilter>();

            AddDal(services);
            AddServices(services);
            AddValidators(services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services
                .AddSingleton<ILayersService, LayersService>()
                .AddSingleton<IMapsService, MapsService>()
                .AddSingleton<IUsersService, UsersService>();
        }

        private static void AddValidators(IServiceCollection services)
        {
            services
                .AddTransient<IValidator<SaveLayerRequest>, SaveLayerRequestValidator>()
                .AddTransient<IValidator<SaveMapRequest>, SaveMapRequestValidator>();
        }

        private static void UseMigration(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var contextFactories = scope.ServiceProvider.GetServices<IDbContextFactory>();
            foreach (var contextFactory in contextFactories)
            {
                using var context = contextFactory.Create();
                context.Database.EnsureCreated();
            }
        }

        private void AddDal(IServiceCollection services)
        {
            services
                .AddUnitOfWorkFactory<EfUnitOfWorkFactory>()
                .AddDbContextFactory<ThematicMapDbContext>(DbTag.App,
                    builder => builder.UseSqlite(Configuration.GetConnectionString("ThematicMapDb")))
                .AddRepository<IUsersRepository, UsersRepository>()
                .AddRepository<IMapsRepository, MapsRepository>()
                .AddRepository<ILayersRepository, LayersRepository>();
        }
    }
}
