using Dapper;
using PAN.API.Application.Interfaces;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Dapper;
using System;
using System.Threading.Tasks;

namespace PAN.API.Infrastructure.Repositories;

public class RawResponseRepository : IRawResponseRepository
{
    private readonly DapperContext _context;

    public RawResponseRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task InsertAsync(PanResponseJson e)
    {
        var sql = @"INSERT INTO pan_response_json 
(correlation_id,pan_verification_id,encrypted_raw_response_json,created_at)
VALUES (@CorrelationId,@PanVerificationId,@EncryptedRawResponseJson,@CreatedAt)";

        using var db = _context.CreateConnection();
        await db.ExecuteAsync(sql, e);
    }
}