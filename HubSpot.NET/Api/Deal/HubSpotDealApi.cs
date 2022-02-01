namespace HubSpot.NET.Api.Deal
{
    using System;
	using System.Linq;
    using System.Net;
    using Flurl;
    using HubSpot.NET.Api.Deal.Dto;
    using HubSpot.NET.Api.Shared;
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.Abstracts;
    using HubSpot.NET.Core.Interfaces;
    using RestSharp;

    public class HubSpotDealApi : ApiRoutable, IHubSpotDealApi
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
        public DealHubSpotModel Create(DealHubSpotModel entity)
        {
            NameTransportModel<DealHubSpotModel> model = new NameTransportModel<DealHubSpotModel>();
            model.ToPropertyTransportModel(entity);

            return _client.Execute<DealHubSpotModel,NameTransportModel<DealHubSpotModel>>(GetRoute<DealHubSpotModel>(), model, Method.POST);
        }

        /// <summary>
        /// Gets a single deal by ID
        /// </summary>
        /// <param name="dealId">ID of the deal</param>
        /// <typeparam name="T">Implementation of DealHubSpotModel</typeparam>
        /// <returns>The deal entity or null if the deal does not exist.</returns>
        public DealHubSpotModel GetById(long dealId)
        {
            try
            {
                return _client.Execute<DealHubSpotModel>(GetRoute<DealHubSpotModel>(dealId.ToString()));
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
        public DealHubSpotModel Update(DealHubSpotModel entity)
        {
            if (entity.Id < 1)
                throw new ArgumentException("Deal entity must have an id set!");

            return _client.Execute<DealHubSpotModel, DealHubSpotModel>(GetRoute<DealHubSpotModel>(entity.Id.ToString()), entity, method: Method.PUT);            
        }

        /// <summary>
        /// Gets a list of deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealListHubSpotModel<DealHubSpotModel> List(bool includeAssociations, ListRequestOptions opts = null)
        {
            opts = opts ?? new ListRequestOptions(250);         

            Url path = GetRoute<DealListHubSpotModel<DealHubSpotModel>>("deal", "paged").SetQueryParam("limit", opts.Limit);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("offset", opts.Offset);

            if (includeAssociations)
                path = path.SetQueryParam("includeAssociations", "true");

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            var data = _client.Execute<DealListHubSpotModel<DealHubSpotModel>, ListRequestOptions>(path, opts);
            return data;
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
        public DealListHubSpotModel<DealHubSpotModel> ListAssociated(bool includeAssociations, long hubId, ListRequestOptions opts = null, string objectName = "contact")
        {
            opts = opts ?? new ListRequestOptions();            

            Url path = $"{GetRoute<DealListHubSpotModel<DealHubSpotModel>>()}/deal/associated/{objectName}/{hubId}/paged"
                .SetQueryParam("limit", opts.Limit);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("offset", opts.Offset);

            if (includeAssociations)
                path = path.SetQueryParam("includeAssociations", "true");

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            return _client.Execute<DealListHubSpotModel<DealHubSpotModel>, ListRequestOptions>(path, opts);
        }

        /// <summary>
        /// Deletes a given deal (by ID)
        /// </summary>
        /// <param name="dealId">ID of the deal</param>
        public void Delete(long dealId)
            => _client.ExecuteOnly(GetRoute<DealHubSpotModel>(dealId.ToString()), method: Method.DELETE);

        /// <summary>
        /// Gets a list of recently created deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealRecentListHubSpotModel<DealHubSpotModel> RecentlyCreated(DealRecentRequestOptions opts = null)
        {
            opts = opts ?? new DealRecentRequestOptions();            

            Url path = $"{GetRoute<DealRecentListHubSpotModel<DealHubSpotModel>>()}/deal/recent/created"
                .SetQueryParam("limit", opts.Limit);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("offset", opts.Offset);

            if (opts.IncludePropertyVersion)
                path = path.SetQueryParam("includePropertyVersions", "true");

            if (!string.IsNullOrEmpty(opts.Since))
                path = path.SetQueryParam("since", opts.Since);

            var data = _client.Execute<DealRecentListHubSpotModel<DealHubSpotModel>, DealRecentRequestOptions>(path, opts);            

            return data;
        }

        /// <summary>
        /// Gets a list of recently modified deals
        /// </summary>
        /// <typeparam name="T">Implementation of DealListHubSpotModel</typeparam>
        /// <param name="opts">Options (limit, offset) relating to request</param>
        /// <returns>List of deals</returns>
        public DealRecentListHubSpotModel<DealHubSpotModel> RecentlyUpdated(DealRecentRequestOptions opts = null)
        {
            opts = opts ?? new DealRecentRequestOptions();            

            var path = $"{GetRoute<DealRecentListHubSpotModel<DealHubSpotModel>>()}/deal/recent/modified"
                .SetQueryParam("limit", opts.Limit);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("offset", opts.Offset);

            if (opts.IncludePropertyVersion)
                path = path.SetQueryParam("includePropertyVersions", "true");

            if (!string.IsNullOrEmpty(opts.Since))
                path = path.SetQueryParam("since", opts.Since);

            var data = _client.Execute<DealRecentListHubSpotModel<DealHubSpotModel>, DealRecentRequestOptions>(path, opts);

            return data;
        }

        /// <summary>
        /// Gets a list of deals based on a search criteria
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="opts">Options (limit, offset) and search criteria relating to request</param>
        /// <returns>List of deals</returns>
        public SearchHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : DealHubSpotModel, new()
        {
            if (opts == null)
            {
                opts = new SearchRequestOptions();
            }

            var path = "/crm/v3/objects/deals/search";

            var data = _client.ExecuteList<SearchHubSpotModel<T>>(path, opts, Method.POST);

            return data;
        }

        /// <summary>
        /// Associate a Company to a deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="DealHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to associate the company with</param>
        /// <param name="companyId">The Id of the company to associate the deal with</param>
        public T AssociateToCompany<T>(T entity, long companyId) where T : DealHubSpotModel, new()
        {
            var path = "/crm-associations/v1/associations";

            _client.Execute(path, new
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
        public T AssociateToContact<T>(T entity, long contactId) where T : DealHubSpotModel, new()
        {
            var path = "/crm-associations/v1/associations";

            _client.Execute(path, new
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
        public T GetAssociations<T>(T entity) where T : DealHubSpotModel, new()
        {
            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var companyPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/5";
            long? offSet = null;

            var companyResults = new List<long>();
            do
            {
                var companyAssociations = _client.ExecuteList<AssociationIdListHubSpotModel>(string.Format("{0}?limit=100{1}", companyPath, offSet == null ? null : "&offset=" + offSet), convertToPropertiesSchema: false);
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
                var contactAssociations = _client.ExecuteList<AssociationIdListHubSpotModel>(string.Format("{0}?limit=100{1}", contactPath, offSet == null ? null : "&offset=" + offSet), convertToPropertiesSchema: false);
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
