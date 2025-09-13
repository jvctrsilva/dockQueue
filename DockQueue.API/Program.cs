using DockQueue.Infra.Ioc;
using Microsoft.OpenApi.Models;
using DockQueue.Domain.Validation;

var builder = WebApplication.CreateBuilder(args);

// Configuração do CORS para permitir requisições do React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Configuração de serviços
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

// Infraestrutura (seu método de IoC)
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

// Middleware de tratamento global de exceções
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        if (exception is DomainExceptionValidation)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message }));
        }
    });
    /*     else if (exception is UnauthorizedException)
         {
             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
             await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message }));
         }
         else if (exception is NotFoundException)
         {
             context.Response.StatusCode = StatusCodes.Status404NotFound;
             await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { error = exception.Message }));
         }
         else
         {
             context.Response.StatusCode = StatusCodes.Status500InternalServerError;
             await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { error = "Erro interno do servidor" }));
         }
     });*/
});

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
