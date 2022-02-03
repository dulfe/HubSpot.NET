using HubSpot.NET.Api.Deal.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotApi
         : IHubSpotApi<Api.Company.Dto.CompanyHubSpotModel, Api.Contact.Dto.ContactHubSpotModel, Api.Deal.Dto.DealHubSpotModel>
    {
    }

    public interface IHubSpotApi<TCompanyHubSpotModel, TContactHubSpotModel, TDealHubSpotModel>
        where TCompanyHubSpotModel : Api.Company.Dto.CompanyHubSpotModel, new()
        where TContactHubSpotModel : Api.Contact.Dto.ContactHubSpotModel, new()
        where TDealHubSpotModel : Api.Deal.Dto.DealHubSpotModel, new()
    {
        IHubSpotCompanyApi<TCompanyHubSpotModel> Company { get; }
        IHubSpotContactApi<TContactHubSpotModel> Contact { get; }
        IHubSpotContactPropertyApi ContactProperty { get; }
        IHubSpotDealApi<TDealHubSpotModel> Deal { get; }
        IHubSpotEngagementApi Engagement { get; }
        IHubSpotCosFileApi File { get; }
        IHubSpotOwnerApi Owner { get; }
        IHubSpotCompanyPropertiesApi CompanyProperties { get; }
        IHubSpotCompanyPropertyGroupsApi CompanyPropertyGroups { get; }
        IHubSpotEmailSubscriptionsApi EmailSubscriptions { get; }
        IHubSpotPipelineApi Pipelines { get; }
    }
}