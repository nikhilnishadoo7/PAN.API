using PAN.API.Application.Services.Interfaces;
using PAN.API.Infrastructure.Providers.Interfaces;
using PAN.API.Infrastructure.Repositories.Interfaces;

namespace PAN.API.Application.Services.Implementations;

public class ProviderFallbackService : IFallbackService
{
    private readonly IEnumerable<IProviderService> _providers;
    private readonly IMasterRepository _masterRepository;

    public ProviderFallbackService(
        IEnumerable<IProviderService> providers,
        IMasterRepository masterRepository)
    {
        _providers = providers;
        _masterRepository = masterRepository;
    }

    public async Task<(bool success, object? response)> ExecuteAsync(string pan)
    {
        Console.WriteLine("Providers count: " + _providers.Count());

        var correlationId = Guid.NewGuid().ToString();

        foreach (var provider in _providers)
        {
            Console.WriteLine("Trying provider: " + provider.ProviderName);

            var master = await _masterRepository.GetByProviderName(provider.ProviderName);

            if (master == null)
            {
                Console.WriteLine("Provider config NOT found in DB");
                continue;
            }

            try
            {
                var (response, raw) = await provider.VerifyAsync(pan, master, correlationId);

                return (true, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Provider failed: {provider.ProviderName}");
                Console.WriteLine(ex.ToString());
            }
        }

        throw new Exception("All providers failed");
    }
}