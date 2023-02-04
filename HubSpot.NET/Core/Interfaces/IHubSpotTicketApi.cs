using HubSpot.NET.Api;
using HubSpot.NET.Api.Ticket.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotTicketApi
    {
        T AssociateToCompany<T>(T entity, long companyId) where T : TicketHubSpotModel, new();
        T AssociateToContact<T>(T entity, long contactId) where T : TicketHubSpotModel, new();
        T AssociateToDeal<T>(T entity, long dealId) where T : TicketHubSpotModel, new();

        T Create<T>(T entity) where T : TicketHubSpotModel, new();
        void Delete(long ticketId);
        void DeleteCompanyAssociation(long ticketId, long companyId);
        void DeleteContactAssociation(long ticketId, long contactId);
        void DeleteDealAssociation(long ticketId, long dealId);
        T GetAssociations<T>(T entity) where T : TicketHubSpotModel, new();
        T GetById<T>(long ticketId) where T : TicketHubSpotModel, new();
        TicketListHubSpotModel<T> List<T>(ListRequestOptionsV3 opts = null) where T : TicketHubSpotModel, new();
        TicketListHubSpotModel<T> ListAssociated<T>(bool includeAssociations, long ticketId, ListRequestOptions opts = null) where T : TicketHubSpotModel, new();
        SearchHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : TicketHubSpotModel, new();
        T Update<T>(T entity) where T : TicketHubSpotModel, new();
    }
}