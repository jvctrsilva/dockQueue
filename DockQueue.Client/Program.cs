using CurrieTechnologies.Razor.SweetAlert2;
using DockQueue.Client.Services;
using DockQueue.Client.Services.UI;
using DockQueue.Settings;
using DockQueue.Client.ViewModels;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


builder.Services.AddAuthenticationCore();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();

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
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval       = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval   = TimeSpan.FromMinutes(20);

});

builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<UserPermissionsService>();
builder.Services.AddScoped<AuthViewModel>();
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<AuthService>();


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