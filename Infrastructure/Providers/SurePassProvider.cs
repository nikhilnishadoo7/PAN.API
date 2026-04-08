using Newtonsoft.Json;
using PAN.API.Application.DTOs.Common;
using PAN.API.Application.Interfaces;
using PAN.API.Application.Mappers;
using PAN.API.Domain.Entities;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PAN.API.Infrastructure.Providers;

public class SurePassProvider : IProviderService
{
    private readonly HttpClient _client;

    public string ProviderName => "Surepass";

    public SurePassProvider(IHttpClientFactory factory)
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

        req.Headers.Add("Authorization", $"Bearer {m.ApiKey}");
        req.Headers.Add("X-Request-Id", correlationId);

        req.Content = new StringContent(
            JsonConvert.SerializeObject(new { pan_number = pan }),
            Encoding.UTF8,
            "application/json"
        );

        var res = await _client.SendAsync(req);
        var json = await res.Content.ReadAsStringAsync();

        Console.WriteLine("SurePass Response: " + json);

        if (!res.IsSuccessStatusCode)
            throw new Exception($"SurePass failed: {json}");

        return (ProviderMapper.MapSurePass(json), json);
    }
}