using System.Threading.Tasks;

namespace PAN.API.Application.Interfaces;

public interface IFallbackService
{
    Task<(bool success, object? response)> ExecuteAsync(string pan);
}