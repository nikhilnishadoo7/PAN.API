using PAN.API.Application.DTOs.Common;
using PAN.API.Application.DTOs.Request;
using PAN.API.Application.Services.Interfaces;
using PAN.API.Application.Utilities;
using PAN.API.Domain.Entities;
using PAN.API.Infrastructure.Repositories.Implementations;
using PAN.API.Infrastructure.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace PAN.API.Application.Services.Implementations;

public class PanVerificationService : IPanVerificationService
{
    private readonly IPanRepository _panRepository;
    private readonly IRawResponseRepository _rawRepository;
    private readonly IFallbackService _fallbackService;
    private readonly IMasterRepository _masterRepository;

    public PanVerificationService(
        IPanRepository panRepository,
        IRawResponseRepository rawRepository,
        IFallbackService fallbackService,
        IMasterRepository masterRepository)
    {
        _panRepository = panRepository;
        _rawRepository = rawRepository;
        _fallbackService = fallbackService;
        _masterRepository = masterRepository;
    }

    public async Task<object> VerifyAsync(PanRequest request, string correlationId, string ip)
    {
        var pan = request.Pan;

        SafeLogger.App($"STEP 1 - Incoming PAN: {pan}");

        if (string.IsNullOrWhiteSpace(pan))
            throw new Exception("PAN is NULL or EMPTY");

        var hash = HashHelper.ComputeSha256(pan);

        SafeLogger.App("STEP 2 - Checking existing PAN");

        var existing = await _panRepository.GetByHash(hash);
        if (existing != null)
        {
            SafeLogger.App("STEP 2.1 - Cache hit");
            return existing;
        }

        SafeLogger.App("STEP 3 - Calling provider");

        var (success, response) = await _fallbackService.ExecuteAsync(pan);

        if (!success || response == null)
            throw new Exception("PAN verification failed");

        if (response is not PanCommonResponseDto res)
            throw new Exception("Mapping failed");

        SafeLogger.App($"STEP 4 - Provider response: {res.Pan}");

        var master = await _masterRepository.GetByProviderName("Surepass");

        if (master == null)
            throw new Exception("Provider config not found");

        var entity = new PanVerification
        {
            Id = Guid.NewGuid(),
            MasterId = master.Id,
            ProviderRequestId = res?.client_id,
            PanHash = hash,
            EncryptedPan = res?.Pan ?? pan,
            PanStatus = res.PanStatus,
            PanLookUpStatus = res.IsSuccess ? "SUCCESS" : "FAILED",
            EncryptedFullName = res.FullName,
            PanCardType = res.Category ?? "person",
            IsPanAadhaarLinked = res.AadhaarLinked,
            CorrelationId = correlationId,
            CallerIp = ip,
            CreatedAt = DateTime.UtcNow
        };

        SafeLogger.App("STEP 5 - Saving verification");

        await _panRepository.Insert(entity);

        SafeLogger.App("STEP 6 - Saving raw response");

        await _rawRepository.InsertAsync(new PanResponseJson
        {
            CorrelationId = correlationId,
            PanVerificationId = entity.Id,
            EncryptedRawResponseJson = response.ToString(),
            CreatedAt = DateTime.UtcNow
        });

        SafeLogger.App("STEP 7 - Completed");

        return response;
    }
}