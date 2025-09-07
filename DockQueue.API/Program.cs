using DockQueue.Infra.Ioc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Configura��o de servi�os
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DockQueue.API",
        Version = "v1"
    });
});

// Infraestrutura (seu m�todo de IoC)
builder.Services.AddInfrastructure(builder.Configuration);

// Endpoints API Explorer (para Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
