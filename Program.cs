using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using PAN.API.Application.Services.Implementations;
using PAN.API.Application.Services.Interfaces;
using PAN.API.Infrastructure.Dapper;
using PAN.API.Infrastructure.Providers.Implementations;
using PAN.API.Infrastructure.Providers.Interfaces;
using PAN.API.Infrastructure.Repositories.Implementations;
using PAN.API.Infrastructure.Repositories.Interfaces;
using PAN.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 🔥 Dapper mapping fix (snake_case → PascalCase)
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// 🔥 Disable automatic 400 validation
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

// 🔥 Serilog Config
LoggerConfig.ConfigureLogger();
builder.Host.UseSerilog();

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

// DB Context
builder.Services.AddSingleton<DapperContext>();

// Repositories
builder.Services.AddScoped<IPanRepository, PanRepository>();
builder.Services.AddScoped<IRawResponseRepository, RawResponseRepository>();
builder.Services.AddScoped<IMasterRepository, MasterRepository>();

// Providers
builder.Services.AddScoped<IProviderService, SurePassProvider>();
builder.Services.AddScoped<IProviderService, SprintVerifyProvider>();

// Services
builder.Services.AddScoped<IFallbackService, ProviderFallbackService>();
builder.Services.AddScoped<IPanVerificationService, PanVerificationService>();

// Background Worker
builder.Services.AddHostedService<BackgroundQueueService>();

var app = builder.Build();


// ================= PIPELINE (VERY IMPORTANT ORDER) =================

// ✅ 1. CorrelationId FIRST
app.UseMiddleware<CorrelationIdMiddleware>();

// ✅ 2. Serilog Request Logging WITH correlation enrichment
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        var correlationId = httpContext.Items["CorrelationId"]?.ToString();

        if (!string.IsNullOrEmpty(correlationId))
        {
            diagnosticContext.Set("correlation_id", correlationId);
        }
    };
});

// ✅ 3. Global Exception Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PAN API v1");
});

// Optional HTTPS
// app.UseHttpsRedirection();

// Auth
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();