using CurrieTechnologies.Razor.SweetAlert2;
using DockQueue.Client.Handlers;
using DockQueue.Client.Services;
using DockQueue.Client.Services.UI;
using DockQueue.Client.Settings;
using DockQueue.Client.ViewModels;
using DockQueue.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IAuthorizationHandler, ScreenAuthorizationHandler>();

builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore(options =>
{
    foreach (Screen s in Enum.GetValues(typeof(Screen)))
    {
        if (s == Screen.None) continue;
        options.AddPolicy($"Screen:{s}", p => p.Requirements.Add(new ScreenRequirement(s)));
    }
});
builder.Services.AddAuthenticationCore();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();
builder.Services.AddTransient<AuthMessageHandler>();


builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(option =>
{
    option.DetailedErrors = true;
    option.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);

});

builder.Services.AddSweetAlert2();
builder.Services.AddWMBOS();
builder.Services.AddWMBSC();

builder.Services.AddApplicationServices();
builder.Services.AddSessionAndCaching();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";


builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
    .AddHttpMessageHandler<AuthMessageHandler>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval       = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval   = TimeSpan.FromMinutes(20);

});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BoxService>();
builder.Services.AddScoped<StatusesService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<IPermissionsApi, PermissionsApi>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<StatusesService>();


builder.Services.AddSingleton<IAuthorizationHandler, ScreenAuthorizationHandler>();
builder.Services.AddScoped<PermissionEditorViewModel>();
builder.Services.AddScoped<AuthViewModel>();
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<BoxViewModel>();
builder.Services.AddScoped<UserViewModel>();
builder.Services.AddScoped<PermissionEditorViewModel>();
builder.Services.AddScoped<StatusListViewModel>();
builder.Services.AddScoped<StatusEditViewModel>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();