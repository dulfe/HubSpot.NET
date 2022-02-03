namespace HubSpot.NET.Api.Deal
{
    using HubSpot.NET.Api.Deal.Dto;
    using HubSpot.NET.Api.Shared;
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.Abstracts;
    using HubSpot.NET.Core.Interfaces;
    using RestSharp;
    using System;
	using System.Collections.Generic;
	using System.Linq;
    using System.Net;

    public class HubSpotDealApi : HubSpotDealApi<DealHubSpotModel>, IHubSpotDealApi
    {
        public HubSpotDealApi(IHubSpotClient client)
            : base(client)
        { }
    }

    public class HubSpotDealApi<TDeal> : ApiRoutable, IHubSpotDealApi<TDeal>
        where TDeal : DealHubSpotModel, new()
    {
        private readonly IHubSpotClient _client;
        public override string MidRoute => "/deals/v1";
        public HubSpotDealApi(IHubSpotClient client)
        {
            _client = client;
            AddRoute<DealHubSpotModel>("/deal");
        }

        /// <summary>
        /// Creates a deal entity
        /// </summary>
        /// <typeparam name="T">Implementation of DealHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        public TDeal Create(TDeal entity)
        {
            NameTransportModel<TDeal> model = new NameTransportModel<TDeal>();
            model.ToPropertyTransportModel(entity);

            return _client.Execute<TDeal, NameTransportModel<TDeal>>(GetRoute<TDeal>(), model, Method.POST);
        }

        /// <summary>
        /// Gets a single deal by ID
        /// </summary>
        /// <param name="dealId">ID of the deal</param>
        /// <typeparam name="T">Implementation of DealHubSpotModel</typeparam>
        /// <returns>The deal entity or null if the deal does not exist.</returns>
        public TDeal GetById(long dealId)
        {
            try
            {
                return _client.Execute<TDeal>(GetRoute<TDeal>(dealId.ToString()));
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Updates a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of DealHubSpotModel</typeparam>
        /// <param name="entity">The deal entity</param>
        /// <returns>The updated deal entity</returns>
        public TDeal Update(TDeal entity)
        {
            if (entity.Id < 1)
                throw new ArgumentException("Deal entity must have an id set!");

            NameTransportModel<TDeal> model = new NameTransportModel<TDeal>();
            model.ToPropertyTransportModel(entity);

            return _client.Execute<TDeal, NameTransportModel<TDeal>>(GetRoute<TDeal>(entity.Id.ToString()), model, method: Method.PUT);
        }

        /// <summary>
        /// Gets a list of deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealListHubSpotModel<TDeal> List(bool includeAssociations, ListRequestOptions opts = null)
        {
            opts = opts ?? new ListRequestOptions(250);

            string path = GetRoute<DealListHubSpotModel<TDeal>>("deal", "paged");

            path += $"{QueryParams.LIMIT}={opts.Limit}";

            if (opts.Offset.HasValue)
                path += $"{QueryParams.OFFSET}={opts.Offset}";

            if (includeAssociations)
                path += $"{QueryParams.INCLUDE_ASSOCIATIONS}=true";

            if (opts.PropertiesToInclude.Any())
                path += $"{QueryParams.PROPERTIES}={opts.PropertiesToInclude}";

            return _client.Execute<DealListHubSpotModel<TDeal>, ListRequestOptions>(path, opts);
        }

        /// <summary>
        /// Gets a list of deals associated to a hubSpot Object
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="includeAssociations">Bool include associated Ids</param>
        /// <param name="hubId">Long Id of Hubspot object related to deals</param>
        /// <param name="objectName">String name of Hubspot object related to deals (contact\account)</param>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealListHubSpotModel<TDeal> ListAssociated(bool includeAssociations, long hubId, ListRequestOptions opts = null, string objectName = "contact")
        {
            opts = opts ?? new ListRequestOptions();

            string path = GetRoute<DealListHubSpotModel<TDeal>>("deal", "associated", $"{objectName}", $"{hubId}", "paged");

            path += $"{QueryParams.LIMIT}={opts.Limit}";

            if (opts.Offset.HasValue)
                path += $"{QueryParams.OFFSET}={opts.Offset}";

            if (includeAssociations)
                path += $"{QueryParams.INCLUDE_ASSOCIATIONS}=true";

            if (opts.PropertiesToInclude.Any())
                path += $"{QueryParams.PROPERTIES}={opts.PropertiesToInclude}";

            return _client.Execute<DealListHubSpotModel<TDeal>, ListRequestOptions>(path, opts);
        }

        /// <summary>
        /// Deletes a given deal (by ID)
        /// </summary>
        /// <param name="dealId">ID of the deal</param>
        public void Delete(long dealId)
            => _client.ExecuteOnly(GetRoute<TDeal>(dealId.ToString()), method: Method.DELETE);

        /// <summary>
        /// Gets a list of recently created deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealRecentListHubSpotModel<TDeal> RecentlyCreated(DealRecentRequestOptions opts = null)
        {
            opts = opts ?? new DealRecentRequestOptions();

            string path = $"{GetRoute<DealRecentListHubSpotModel<TDeal>>()}/deal/recent/created";

            path += $"{QueryParams.LIMIT}={opts.Limit}";

            if (opts.Offset.HasValue)
                path += $"{QueryParams.OFFSET}={opts.Offset}";

            if (opts.IncludePropertyVersion)
                path += $"{QueryParams.INCLUDE_PROPERTY_VERSIONS}=true";


            if (!string.IsNullOrEmpty(opts.Since))
                path += $"{QueryParams.SINCE}={opts.Since}";

            var data = _client.Execute<DealRecentListHubSpotModel<TDeal>, DealRecentRequestOptions>(path, opts);            

            return data;
        }

        /// <summary>
        /// Gets a list of recently modified deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealRecentListHubSpotModel<TDeal> RecentlyUpdated(DealRecentRequestOptions opts = null)
        {
            opts = opts ?? new DealRecentRequestOptions();

            string path = GetRoute<DealRecentListHubSpotModel<TDeal>>("deal", "recent", "modified");
            path += $"{QueryParams.LIMIT}={opts.Limit}";

            if (opts.Offset.HasValue)
                path += $"{QueryParams.OFFSET}={opts.Offset}";

            if (opts.IncludePropertyVersion)
                path += $"{QueryParams.INCLUDE_PROPERTY_VERSIONS}=true";

            if (!string.IsNullOrEmpty(opts.Since))
                path += $"{QueryParams.SINCE}={opts.Since}";

            return _client.Execute<DealRecentListHubSpotModel<TDeal>, DealRecentRequestOptions>(path, opts);
        }

        /// <summary>
        /// Gets a list of deals based on a search criteria
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="opts">Options (limit, offset) and search criteria relating to request</param>
        /// <returns>List of deals</returns>
        public SearchHubSpotModel<TDeal> Search(SearchRequestOptions opts = null)
        {
            if (opts == null)
            {
                opts = new SearchRequestOptions();
            }

            var path = "/crm/v3/objects/deals/search";

            var data = _client.Execute<SearchHubSpotModel<TDeal>, SearchRequestOptions>(path, opts, Method.POST);

            return data;
        }

        /// <summary>
        /// Associate a Company to a deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to associate the company with</param>
        /// <param name="companyId">The Id of the company to associate the deal with</param>
        public TDeal AssociateToCompany(TDeal entity, long companyId)
        {
            var path = "/crm-associations/v1/associations";

            _client.Execute<TDeal, object>(path, new
            {
                fromObjectId = entity.Id,
                toObjectId = companyId,
                category = "HUBSPOT_DEFINED",
                definitionId = 5 // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            }, method: Method.PUT);
            entity.Associations.AssociatedCompany = new[] { companyId };
            return entity;
        }

        /// <summary>
        /// Associate a Cntact to a deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to associate the contact with</param>
        /// <param name="contactId">The Id of the contact to associate the deal with</param>
        public TDeal AssociateToContact(TDeal entity, long contactId)
        {
            var path = "/crm-associations/v1/associations";

            _client.Execute<TDeal, object>(path, new
            {
                fromObjectId = entity.Id,
                toObjectId = contactId,
                category = "HUBSPOT_DEFINED",
                definitionId = 3 // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            }, method: Method.PUT);
            entity.Associations.AssociatedContacts = new[] { contactId };
            return entity;
        }

        /// <summary>
        /// Gets a list of associations for a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to get associations for</param>
        public TDeal GetAssociations(TDeal entity)
        {
            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var companyPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/5";
            long? offSet = null;

            var companyResults = new List<long>();
            do
            {
                var companyAssociations = _client.Execute<AssociationIdListHubSpotModel>(string.Format("{0}?limit=100{1}", companyPath, offSet == null ? null : "&offset=" + offSet));
                if (companyAssociations.Results.Any())
                    companyResults.AddRange(companyAssociations.Results);
                if (companyAssociations.HasMore)
                    offSet = companyAssociations.Offset;
                else
                    offSet = null;
            } while (offSet != null);
            if (companyResults.Any())
                entity.Associations.AssociatedCompany = companyResults.ToArray();
            else
                entity.Associations.AssociatedCompany = null;

            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var contactPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/3";

            var contactResults = new List<long>();
            do
            {
                var contactAssociations = _client.Execute<AssociationIdListHubSpotModel>(string.Format("{0}?limit=100{1}", contactPath, offSet == null ? null : "&offset=" + offSet));
                if (contactAssociations.Results.Any())
                    contactResults.AddRange(contactAssociations.Results);
                if (contactAssociations.HasMore)
                    offSet = contactAssociations.Offset;
                else
                    offSet = null;
            } while (offSet != null);
            if (contactResults.Any())
                entity.Associations.AssociatedContacts = contactResults.ToArray();
            else
                entity.Associations.AssociatedContacts = null;

            return entity;
        }
	}
}
