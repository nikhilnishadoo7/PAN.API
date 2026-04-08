namespace PAN.API.Application.Interfaces;

using PAN.API.Application.DTOs.Common;
using PAN.API.Domain.Entities;
using System.Threading.Tasks;

public interface IProviderService
{
    string ProviderName { get; }

    Task<(PanCommonResponseDto response, string raw)> VerifyAsync(
        string pan,
        PanMaster master,
        string correlationId
    );
}