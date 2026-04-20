using FoodTracker.Api.Configuration;
using FoodTracker.Api.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNotionClient(builder.Configuration);
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddNotionRepositories();
builder.Services.AddAppServices();

builder.Services.AddSingleton<IExceptionHandlerStrategy, ValidationExceptionStrategy>();
builder.Services.AddSingleton<IExceptionHandlerStrategy, UnhandledExceptionStrategy>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodTracker API", Version = "v1" }));

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FoodTracker API v1"));

app.UseHttpsRedirection();
app.MapControllers();
app.Run();