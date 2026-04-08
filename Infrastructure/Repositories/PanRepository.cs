using Dapper;
using PAN.API.Application.Interfaces;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Dapper;
using System;
using System.Threading.Tasks;

namespace PAN.API.Infrastructure.Repositories;

public class PanRepository : IPanRepository
{
    private readonly DapperContext _context;

    public PanRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PanVerification?> GetByHash(string hash)
    {
        var sql = "SELECT * FROM pan_verification WHERE pan_hash=@hash LIMIT 1";

        using var db = _context.CreateConnection();

        return await db.QueryFirstOrDefaultAsync<PanVerification>(sql, new { hash });
    }

    public async Task<Guid> Insert(PanVerification e)
    {
        if (e == null)
            throw new ArgumentNullException(nameof(e));

        var sql = @"INSERT INTO pan_verification 
(Id,correlation_id,master_id,pan_hash,encrypted_pan,pan_status,pan_lookup_status,
encrypted_full_name,pan_card_type,is_pan_aadhaar_linked,caller_ip,created_at)
VALUES (@Id,@CorrelationId,@MasterId,@PanHash,@EncryptedPan,@PanStatus,@PanLookUpStatus,
@EncryptedFullName,@PanCardType,@IsPanAadhaarLinked,@CallerIp,@CreatedAt)";

        using var db = _context.CreateConnection();

        await db.ExecuteAsync(sql, e);

        return e.Id;
    }
}