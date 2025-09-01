using DockQueue.Infrastructure;
using DockQueue.Application.Services;
using DockQueue.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext <DockQueueDbContext> (options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped <UserService> ();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints usando o UserService
app.MapGet("/users", async (UserService service) =>
{
    return await service.GetAllUsersAsync();
}).WithName("GetUsers");

app.MapGet("/users/{id:int}", async (int id, UserService service) =>
{
    var user = await service.GetUserByIdAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
}).WithName("GetUserById");

app.MapPost("/users", async (User user, UserService service) =>
{
    var createdUser = await service.CreateUserAsync(user);
    return Results.Created($"/users/{createdUser.Id}", createdUser);
}).WithName("CreateUser");

app.Run();
