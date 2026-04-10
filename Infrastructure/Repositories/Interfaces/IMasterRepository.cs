using PAN.API.Domain.Entities;

namespace PAN.API.Infrastructure.Repositories.Interfaces;

public interface IMasterRepository
{
    Task<PanMaster?> GetByProviderName(string providerName);
}