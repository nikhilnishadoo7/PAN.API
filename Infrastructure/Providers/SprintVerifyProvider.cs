using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PAN.API.Application.Interfaces;
using PAN.API.Application.DTOs.Common;
using PAN.API.Application.Mappers;
using PAN.API.Domain.Entities;

namespace PAN.API.Infrastructure.Providers;

public class SprintVerifyProvider : IProviderService
{
    private readonly HttpClient _client;

    public string ProviderName => "SprintVerify";

    public SprintVerifyProvider(IHttpClientFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task<(PanCommonResponseDto response, string raw)> VerifyAsync(
        string pan,
        PanMaster m,
        string correlationId)
    {
        var url = $"{m.BaseUrl}{m.Endpoint}";

        var req = new HttpRequestMessage(HttpMethod.Post, url);

        req.Headers.Add("Authorization", "Bearer MOCK_TOKEN");
        req.Headers.Add("X-Request-Id", correlationId);

        req.Content = new StringContent(
            JsonConvert.SerializeObject(new { idNumber = pan }),
            Encoding.UTF8,
            "application/json"
        );

        var res = await _client.SendAsync(req);
        var json = await res.Content.ReadAsStringAsync();

        Console.WriteLine("Sprint Response: " + json);

        if (!res.IsSuccessStatusCode)
            throw new Exception($"Sprint failed: {json}");

        return (ProviderMapper.MapSprint(json), json);
    }
}