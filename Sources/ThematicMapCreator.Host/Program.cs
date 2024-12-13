using System.Threading.Tasks;
using Core.Dal.EntityFramework.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
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

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddSingleton<TmcExceptionFilter>();
        builder.Services.AddControllers(options => options.Filters.AddService<TmcExceptionFilter>());

        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();

        builder.Services.AddDal(builder.Configuration);
        builder.Services.AddServices();

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();
        app.MapHub<NotificationHub>("/hub/notification", options => options.AllowStatefulReconnects = true);

        await MigrateDatabaseAsync(app);

        await app.RunAsync();
    }

    private static void AddDal(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddEfUnitOfWorkFactory<ThematicMapDbContext>(builder => builder.UseSqlite(configuration.GetConnectionString("ThematicMapDb")))
            .AddRepository<IUsersRepository, UsersRepository>()
            .AddRepository<IMapsRepository, MapsRepository>()
            .AddRepository<ILayersRepository, LayersRepository>();

    private static void AddServices(this IServiceCollection services)
    {
        services.AddMediatR(configuration => configuration
            .RegisterServicesFromAssemblyContaining<MapSaveCommand>()
            .RegisterServicesFromAssemblyContaining(typeof(SignalRNotificationHandler<>)));

        services.AddTransient<IValidator<MapSaveCommand>, MapSaveCommandValidator>();
    }

    private static async Task MigrateDatabaseAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ThematicMapDbContext>>();
        await using var context = await contextFactory.CreateDbContextAsync();
        await context.Database.MigrateAsync();
    }
}
