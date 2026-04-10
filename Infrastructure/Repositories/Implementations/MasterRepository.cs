using Dapper;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Dapper;
using PAN.API.Infrastructure.Repositories.Interfaces;

namespace PAN.API.Infrastructure.Repositories.Implementations;

public class MasterRepository : IMasterRepository
{
    private readonly DapperContext _context;

    public MasterRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PanMaster?> GetByProviderName(string providerName)
    {
        var sql = @"
        SELECT 
            id,
            provider_name AS ProviderName,
            provider_baseurl AS BaseUrl,
            provider_endpoint AS Endpoint,
            encrypted_api_key AS ApiKey,
            priority AS Priority,
            is_active AS IsActive
        FROM panmaster
       WHERE LOWER(provider_name) = LOWER(@providerName)
        LIMIT 1";

        using var db = _context.CreateConnection();

        return await db.QueryFirstOrDefaultAsync<PanMaster>(sql, new { providerName });
    }
}