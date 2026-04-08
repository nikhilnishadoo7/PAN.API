using PAN.API.Domain.Entities;

namespace PAN.API.Application.Mappers;

public static class EntityMapper
{
    public static PanMaster Map(dynamic db)
    {
        return new PanMaster
        {
            Id = db.id,
            ProviderName = db.provider_name,
            BaseUrl = db.base_url,
            Endpoint = db.endpoint,
            ApiKey = db.api_key,
            Priority = db.priority
        };
    }
}