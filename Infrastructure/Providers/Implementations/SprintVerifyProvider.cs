using Newtonsoft.Json;
using PAN.API.Application.DTOs.Common;
using PAN.API.Application.Mappers;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Providers.Interfaces;
using System.Net.Http;
using System.Text;

namespace PAN.API.Infrastructure.Providers.Implementations;

public class SprintVerifyProvider : IProviderService
{
    private readonly HttpClient _client;

    public string ProviderName => "SprintVerify";

    public SprintVerifyProvider(IHttpClientFactory factory)
    {
        _client = factory.CreateClient("SprintVerifyClient");
    }

    public async Task<(PanCommonResponseDto response, string raw)> VerifyAsync(
        string pan,
        PanMaster m,
        string correlationId)
    {
        var url = $"{m.BaseUrl.TrimEnd('/')}/{m.Endpoint.TrimStart('/')}";

        Console.WriteLine("---- Sprint CALL ----");
        Console.WriteLine("URL: " + url);

        var req = new HttpRequestMessage(HttpMethod.Post, url);

        if (!string.IsNullOrEmpty(m.ApiKey))
        {
            req.Headers.Add("Authorization", $"Bearer {m.ApiKey}");
        }

        req.Headers.Add("X-Request-Id", correlationId);

        req.Content = new StringContent(
            JsonConvert.SerializeObject(new { idNumber = pan }),
            Encoding.UTF8,
            "application/json"
        );

        var res = await _client.SendAsync(req);
        var json = await res.Content.ReadAsStringAsync();

        Console.WriteLine("Sprint Status: " + res.StatusCode);
        Console.WriteLine("Sprint Response: " + json);

        if (!res.IsSuccessStatusCode)
        {
            throw new Exception($"Sprint failed: {json}");
        }

        return (ProviderMapper.MapSprint(json), json);
    }
}