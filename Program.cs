using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using PAN.API.Application.Interfaces;
using PAN.API.Application.Services;
using PAN.API.Infrastructure.Dapper;
using PAN.API.Infrastructure.Providers;
using PAN.API.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Dapper fix
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// 🔥 Disable automatic 400 validation (IMPORTANT)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PAN API",
        Version = "v1"
    });
});

// HttpClients
builder.Services.AddHttpClient("SurepassClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("SprintVerifyClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
// DB
builder.Services.AddSingleton<DapperContext>();

// Repositories
builder.Services.AddScoped<IPanRepository, PanRepository>();
builder.Services.AddScoped<IRawResponseRepository, RawResponseRepository>();
builder.Services.AddScoped<MasterRepository>();

// Providers
builder.Services.AddScoped<IProviderService, SurePassProvider>();
builder.Services.AddScoped<IProviderService, SprintVerifyProvider>();

// Services
builder.Services.AddScoped<IFallbackService, ProviderFallbackService>();
builder.Services.AddScoped<IPanVerificationService, PanVerificationService>();

builder.Services.AddHostedService<BackgroundQueueService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PAN API v1");
});

app.UseAuthorization();
app.MapControllers();

app.Run();