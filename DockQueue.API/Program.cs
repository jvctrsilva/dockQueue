using CurrieTechnologies.Razor.SweetAlert2;
using DockQueue.Settings;
using DockQueue.Services.UI;

var builder = WebApplication.CreateBuilder(args);

// ==== API e Infra ====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(); // se já usa

// JWT/Auth que você já tem:
builder.Services.AddAuthentication(/*...*/);
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
builder.Services.AddHttpClient();

// SweetAlert + suas extensões
builder.Services.AddSweetAlert2();
builder.Services.AddWMBOS();
builder.Services.AddWMBSC();

builder.Services.AddApplicationServices(); // suas DI internas
builder.Services.AddSessionAndCaching();

// Services de UI do front
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<UserPermissionsService>();

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
