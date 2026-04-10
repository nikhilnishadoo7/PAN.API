using Newtonsoft.Json;
using PAN.API.Application.DTOs.Common;
using PAN.API.Application.Mappers;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Providers.Interfaces;
using System.Net.Http;
using System.Text;

namespace PAN.API.Infrastructure.Providers.Implementations;

public class SurePassProvider : IProviderService
{
    private readonly HttpClient _client;

    public string ProviderName => "Surepass";

    public SurePassProvider(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("SurepassClient"); 
    }

    public async Task<(PanCommonResponseDto response, string raw)> VerifyAsync(
        string pan,
        PanMaster m,
        string correlationId)
    {
        var url = $"{m.BaseUrl.TrimEnd('/')}/{m.Endpoint.TrimStart('/')}";

        Console.WriteLine("---- SurePass CALL ----");
        Console.WriteLine("URL: " + url);

        var request = new HttpRequestMessage(HttpMethod.Post, url);

        // ✅ Authorization (safe)
        if (!string.IsNullOrEmpty(m.ApiKey))
        {
            request.Headers.Add("Authorization", $"Bearer {m.ApiKey}");
        }

        request.Headers.Add("X-Request-Id", correlationId);

        request.Content = new StringContent(
            JsonConvert.SerializeObject(new { pan_number = pan }),
            Encoding.UTF8,
            "application/json"
        );

        var res = await _client.SendAsync(request);
        var json = await res.Content.ReadAsStringAsync();

        Console.WriteLine("SurePass Status: " + res.StatusCode);
        Console.WriteLine("SurePass Response: " + json);

        if (!res.IsSuccessStatusCode)
        {
            throw new Exception($"SurePass failed: {json}");
        }

        return (ProviderMapper.MapSurePass(json), json);
    }
}