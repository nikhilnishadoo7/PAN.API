using Dapper;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Dapper;
using System.Threading.Tasks;

namespace PAN.API.Infrastructure.Repositories;

public class MasterRepository
{
    private readonly DapperContext _context;

    public MasterRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PanMaster?> GetByProviderName(string providerName)
    {
        var sql = "SELECT * FROM provider_master WHERE provider_name=@providerName LIMIT 1";

        using var db = _context.CreateConnection();

        return await db.QueryFirstOrDefaultAsync<PanMaster>(sql, new { providerName });
    }
}