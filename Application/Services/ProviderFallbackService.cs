using System;
using System.Threading.Tasks;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Repositories;
using System.Collections.Generic;
using PAN.API.Application.Interfaces;
namespace PAN.API.Application.Services;

public class ProviderFallbackService : IFallbackService
{
    private readonly IEnumerable<IProviderService> _providers;
    private readonly MasterRepository _masterRepository;

    public ProviderFallbackService(
        IEnumerable<IProviderService> providers,
        MasterRepository masterRepository)
    {
        _providers = providers;
        _masterRepository = masterRepository;
    }

    public async Task<(bool success, object? response)> ExecuteAsync(string pan)
    {
        var correlationId = Guid.NewGuid().ToString();

        foreach (var provider in _providers)
        {
            var master = await _masterRepository.GetByProviderName(provider.ProviderName);

            if (master == null)
                continue;

            try
            {
                var (response, raw) = await provider.VerifyAsync(pan, master, correlationId);

                return (true, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Provider failed: {provider.ProviderName} → {ex.Message}");
            }
        }

        throw new Exception("All providers failed");
    }
}