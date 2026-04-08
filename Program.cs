using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PAN.API.Application.Interfaces;
using PAN.API.Application.Services;
using PAN.API.Infrastructure.Dapper;
using PAN.API.Infrastructure.Providers;
using PAN.API.Infrastructure.Repositories;
using System;
using System.IO;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// ✅ FORCE load appsettings.json (FIX)
builder.Host.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
});
// ✅ Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PAN Verification API",
        Version = "v1"
    });
});

// ✅ HttpClients
builder.Services.AddHttpClient("SurepassClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("SprintVerifyClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ✅ DB Context
builder.Services.AddSingleton<DapperContext>();

// ✅ Repositories
builder.Services.AddScoped<IPanRepository, PanRepository>();
builder.Services.AddScoped<IRawResponseRepository, RawResponseRepository>();
builder.Services.AddScoped<MasterRepository>();

// ✅ Providers
builder.Services.AddScoped<IProviderService, SurePassProvider>();
builder.Services.AddScoped<IProviderService, SprintVerifyProvider>();

// ✅ Services
builder.Services.AddScoped<IPanVerificationService, PanVerificationService>();
builder.Services.AddScoped<IFallbackService, ProviderFallbackService>();

builder.Services.AddHostedService<BackgroundQueueService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();