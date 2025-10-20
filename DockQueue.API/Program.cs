using CurrieTechnologies.Razor.SweetAlert2;
using DockQueue.Services;
using DockQueue.Settings;
using DockQueue.Services.UI;
using DockQueue.ViewModels;
using DockQueue.Infra.Ioc;

var builder = WebApplication.CreateBuilder(args);

// ==== API e Infra ====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(); // se já usa

// JWT/Auth
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
builder.Services.AddAuthorization();

// ==== Blazor Server ====
builder.Services.AddRazorPages(); // necessário para _Host.cshtml
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(opt =>
    {
        opt.DetailedErrors = true;
        opt.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
    });

builder.Services.AddSignalR(opt =>
{
    opt.KeepAliveInterval = TimeSpan.FromSeconds(15);
    opt.ClientTimeoutInterval = TimeSpan.FromMinutes(20);
});

builder.Services.AddHttpContextAccessor();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// SweetAlert + suas extensões
builder.Services.AddSweetAlert2();
builder.Services.AddWMBOS();
builder.Services.AddWMBSC();

builder.Services.AddApplicationServices(); // suas DI internas
builder.Services.AddSessionAndCaching();

builder.Services.AddInfrastructure(builder.Configuration);

// Services de UI do front
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<UserPermissionsService>();

// ViewModels e Services MVVM
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<AuthViewModel>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// ==== Pipeline ====
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // necessário para wwwroot (css/js/img) do Blazor

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// API
app.MapControllers();

// Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
