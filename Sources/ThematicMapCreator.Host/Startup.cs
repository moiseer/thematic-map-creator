using Core.Dal.EntityFramework.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ThematicMapCreator.Domain.Commands;
using ThematicMapCreator.Domain.Repositories;
using ThematicMapCreator.Domain.Validators;
using ThematicMapCreator.Host.Filters;
using ThematicMapCreator.Host.Notifications;
using ThematicMapCreator.Host.Persistence.Contexts;
using ThematicMapCreator.Host.Persistence.Repositories;

namespace ThematicMapCreator.Host;

public sealed class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    private IConfiguration Configuration { get; }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSerilogRequestLogging();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<NotificationHub>("/api/notification-hub", options => options.AllowStatefulReconnects = true);
        });

        UseMigration(app);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.AddService<TmcExceptionFilter>());
        services.AddSingleton<TmcExceptionFilter>();

        services.AddSwaggerGen();
        services.AddSignalR();

        AddDal(services);
        AddServices(services);
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration
            .RegisterServicesFromAssemblyContaining<MapSaveCommand>()
            .RegisterServicesFromAssemblyContaining(typeof(SignalRNotificationHandler<>)));
        services.AddTransient<IValidator<MapSaveCommand>, MapSaveCommandValidator>();
    }

    private static void UseMigration(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ThematicMapDbContext>>();
        using var context = contextFactory.CreateDbContext();
        context.Database.EnsureCreated();
    }

    private void AddDal(IServiceCollection services)
    {
        services
            .AddEfUnitOfWorkFactory<ThematicMapDbContext>(builder => builder.UseSqlite(Configuration.GetConnectionString("ThematicMapDb")))
            .AddRepository<IUsersRepository, UsersRepository>()
            .AddRepository<IMapsRepository, MapsRepository>()
            .AddRepository<ILayersRepository, LayersRepository>();
    }
}
