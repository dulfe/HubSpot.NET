﻿using HubSpot.NET.Api;
using HubSpot.NET.Api.Deal.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotDealApi : IHubSpotDealApi<DealHubSpotModel>
    {
    }
    public interface IHubSpotDealApi<T> : ICRUDable<T>
        where T : DealHubSpotModel, IHubSpotModel, new()
    {
        DealListHubSpotModel<T> List(bool includeAssociations, ListRequestOptions opts = null);     
        DealListHubSpotModel<T> ListAssociated(bool includeAssociations, long hubId, ListRequestOptions opts = null, string objectName = "contact");
        DealRecentListHubSpotModel<T> RecentlyCreated(DealRecentRequestOptions opts = null);
        DealRecentListHubSpotModel<T> RecentlyUpdated(DealRecentRequestOptions opts = null);

        SearchHubSpotModel<T> Search(SearchRequestOptions opts = null);

        T AssociateToCompany(T entity, long companyId);

        T AssociateToContact(T entity, long contactId);

        T GetAssociations(T entity);
    }
}