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

            BaseUrl = db.provider_baseurl,    

            Endpoint = db.provider_endpoint,   

            ApiKey = db.encrypted_api_key,     

            Priority = db.priority,

            IsActive = db.is_active
        };
    }
}