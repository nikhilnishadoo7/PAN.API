using Dapper;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Dapper;
using PAN.API.Infrastructure.Repositories.Interfaces;

namespace PAN.API.Infrastructure.Repositories.Implementations;

public class PanRepository : IPanRepository
{
    private readonly DapperContext _context;

    public PanRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PanVerification?> GetByHash(string hash)
    {
        var sql = @"SELECT * FROM panverifications WHERE panhash = @hash LIMIT 1";

        using var db = _context.CreateConnection();
        return await db.QueryFirstOrDefaultAsync<PanVerification>(sql, new { hash });
    }

    public async Task<Guid> Insert(PanVerification e)
    {
        var sql = @"
        INSERT INTO panverifications
        (
            id,
            correlationid,
            masterid,
            providerrequestid,
            panhash,
            encryptedpan,
            panstatus,
            panlookupstatus,
            encryptedfullname,
            pancardtype,
            ispanaadhaarliked,
            callerip,
            createdat
        )
        VALUES
        (
            @Id,
            @CorrelationId,
            @MasterId,
            @ProviderRequestId,
            @PanHash,
            @EncryptedPan,
            @PanStatus,
            @PanLookUpStatus,
            @EncryptedFullName,
            @PanCardType,
            @IsPanAadhaarLinked,
            @CallerIp,
            @CreatedAt
        )";

        using var db = _context.CreateConnection();

        await db.ExecuteAsync(sql, new
        {
            e.Id,
            e.CorrelationId,
            e.MasterId,
            ProviderRequestId = e.ProviderRequestId,
            e.PanHash,
            e.EncryptedPan,
            e.PanStatus,
            e.PanLookUpStatus,
            e.EncryptedFullName,
            e.PanCardType,
            IsPanAadhaarLinked = e.IsPanAadhaarLinked, // map to column name
            e.CallerIp,
            e.CreatedAt
        });

        return e.Id;
    }
}