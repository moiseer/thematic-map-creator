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
using ThematicMapCreator.Domain;
using ThematicMapCreator.Domain.Commands;
using ThematicMapCreator.Domain.Repositories;
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
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            UseMigration(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.AddService<TmcExceptionFilter>());

            services.AddSwaggerGen();

            services.AddSingleton<TmcExceptionFilter>();

            AddDal(services);
            AddServices(services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblyContaining<MapSaveCommand>());
            services.AddTransient<IValidator<MapSaveCommand>, MapSaveCommandValidator>();
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
                .AddDbContextFactory<ThematicMapDbContext>(builder => builder.UseSqlite(Configuration.GetConnectionString("ThematicMapDb")))
                .AddRepository<IUsersRepository, UsersRepository>()
                .AddRepository<IMapsRepository, MapsRepository>()
                .AddRepository<ILayersRepository, LayersRepository>();
        }
    }
}
