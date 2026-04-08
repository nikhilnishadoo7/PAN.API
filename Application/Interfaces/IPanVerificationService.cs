using PAN.API.Application.DTOs.Request;
using System.Threading.Tasks;

namespace PAN.API.Application.Interfaces;

public interface IPanVerificationService
{
    Task<object> VerifyAsync(PanRequest request, string correlationId, string ip);
}