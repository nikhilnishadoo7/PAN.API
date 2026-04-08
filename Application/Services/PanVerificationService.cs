using System;
using System.Threading.Tasks;
using PAN.API.Application.DTOs.Request;
using PAN.API.Application.Interfaces;
using PAN.API.Domain.Entities;

namespace PAN.API.Application.Services;

public class PanVerificationService : IPanVerificationService
{
    private readonly IPanRepository _panRepository;
    private readonly IRawResponseRepository _rawRepository;
    private readonly IFallbackService _fallbackService;

    public PanVerificationService(
        IPanRepository panRepository,
        IRawResponseRepository rawRepository,
        IFallbackService fallbackService)
    {
        _panRepository = panRepository;
        _rawRepository = rawRepository;
        _fallbackService = fallbackService;
    }

    public async Task<object> VerifyAsync(PanRequest request, string correlationId, string ip)
    {
        var pan = request.Pan;

        var hash = pan.GetHashCode().ToString();

        var existing = await _panRepository.GetByHash(hash);

        if (existing != null)
        {
            return existing;
        }

        var (success, response) = await _fallbackService.ExecuteAsync(pan);

        if (!success || response == null)
        {
            throw new Exception("PAN verification failed");
        }

        var entity = new PanVerification
        {
            Id = Guid.NewGuid(),
            PanHash = hash,
            CorrelationId = correlationId,
            CallerIp = ip,
            CreatedAt = DateTime.UtcNow
        };

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