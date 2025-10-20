using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using DockQueue.Authentication;
using DockQueue.Services.UI;

namespace DockQueue.Settings;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ProtectedLocalStorage>();

        services.AddSingleton<AppState>();

        services.AddScoped<IActionService, ActionService>();
        services.AddScoped<StateService>();
        services.AddScoped<MenuDataService>();
        services.AddScoped<NavScrollService>();
        services.AddScoped<SessionService>();
        services.AddScoped<WorkingHoursService>();

        return services;
    }

    public static IServiceCollection AddSessionAndCaching(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddMemoryCache();

        services.AddSession(opts =>
        {
            opts.IdleTimeout = TimeSpan.FromMinutes(30);
            opts.Cookie.HttpOnly = true;
            opts.Cookie.IsEssential = true;
        });
        return services;
    }
}