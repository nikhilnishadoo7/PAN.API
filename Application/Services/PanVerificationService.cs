using System;
using System.Threading.Tasks;
using PAN.API.Application.DTOs.Request;
using PAN.API.Application.Interfaces;
using PAN.API.Domain.Entities;
using PAN.API.Application.DTOs.Common;
using PAN.API.Infrastructure.Repositories;

namespace PAN.API.Application.Services;

public class PanVerificationService : IPanVerificationService
{
    private readonly IPanRepository _panRepository;
    private readonly IRawResponseRepository _rawRepository;
    private readonly IFallbackService _fallbackService;
    private readonly MasterRepository _masterRepository;

    public PanVerificationService(
        IPanRepository panRepository,
        IRawResponseRepository rawRepository,
        IFallbackService fallbackService,
        MasterRepository masterRepository)
    {
        _panRepository = panRepository;
        _rawRepository = rawRepository;
        _fallbackService = fallbackService;
        _masterRepository = masterRepository;
    }

    public async Task<object> VerifyAsync(PanRequest request, string correlationId, string ip)
    {
        var pan = request.Pan;

        Console.WriteLine("STEP 1 - Incoming PAN: " + pan);

        if (string.IsNullOrWhiteSpace(pan))
            throw new Exception("PAN is NULL or EMPTY");

        var hash = pan.GetHashCode().ToString();

        var existing = await _panRepository.GetByHash(hash);
        if (existing != null)
            return existing;

        var (success, response) = await _fallbackService.ExecuteAsync(pan);

        if (!success || response == null)
            throw new Exception("PAN verification failed");

        // 🔥 SAFE CAST
        if (response is not PanCommonResponseDto res)
            throw new Exception("Mapping failed: response is not PanCommonResponseDto");

        Console.WriteLine("STEP 2 - Mapped PAN: " + res.Pan);

        // 🔥 Get MasterId dynamically
        var master = await _masterRepository.GetByProviderName("Surepass");

        if (master == null)
            throw new Exception("Provider config not found");

        var entity = new PanVerification
        {
            Id = Guid.NewGuid(),
            MasterId = master.Id,
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

        Console.WriteLine("STEP 3 - ENTITY PAN: " + entity.EncryptedPan);

        await _panRepository.Insert(entity);

        await _rawRepository.InsertAsync(new PanResponseJson
        {
            CorrelationId = correlationId,
            PanVerificationId = entity.Id,
            EncryptedRawResponseJson = response.ToString(),
            CreatedAt = DateTime.UtcNow
        });

        return response;
    }
}