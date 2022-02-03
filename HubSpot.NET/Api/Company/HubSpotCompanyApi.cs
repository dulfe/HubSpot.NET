namespace HubSpot.NET.Api.Company
{
    using HubSpot.NET.Api.Company.Dto;
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.Abstracts;
    using HubSpot.NET.Core.Interfaces;
    using RestSharp;
    using System;
	using System.Collections.Generic;
	using System.Linq;
    using System.Net;

    public class HubSpotCompanyApi : HubSpotCompanyApi<CompanyHubSpotModel>, IHubSpotCompanyApi
    {
        public HubSpotCompanyApi(IHubSpotClient client)
            :base (client)
        {
        }
    }

	public class HubSpotCompanyApi<TCompany> : ApiRoutable, IHubSpotCompanyApi<TCompany>
        where TCompany : CompanyHubSpotModel, new()
    {
        private readonly IHubSpotClient _client;
        public override string MidRoute => "/companies/v2";        

        public HubSpotCompanyApi(IHubSpotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a Company entity
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public TCompany Create(TCompany entity)
            => _client.Execute<TCompany, TCompany>(GetRoute<TCompany>("companies"), entity, Method.POST);

        /// <summary>
        /// Gets a specific company by it's ID
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="companyId">The ID</param>
        /// <returns>The company entity or null if the company does not exist.</returns>
        public TCompany GetById(long companyId)
        {
            try
            {
                var _data = _client.Execute<TCompany>(GetRoute<TCompany>("companies", companyId.ToString()));

                return _data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Gets a company by domain name
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="domain">Domain name to search for</param>
        /// <param name="options">Set of search options</param>
        /// <returns>The company entity or null if the company does not exist.</returns>
        public CompanySearchResultModel<TCompany> GetByDomain(string domain, CompanySearchByDomain opts = null)
        {
            opts = opts ?? new CompanySearchByDomain();

            var path = GetRoute<TCompany>("domains", domain, "companies");

            try
            {
                var data = _client.Execute<CompanySearchResultModel<TCompany>, CompanySearchByDomain>(path, opts, Method.POST);

                return data;            }
            catch (HubSpotException exception)
        {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public CompanyListHubSpotModel<TCompany> List(ListRequestOptions opts = null)
        {
            opts = opts ?? new ListRequestOptions();

            string path = GetRoute<CompanyHubSpotModel>("companies", "paged");

            path += $"{QueryParams.COUNT}={opts.Limit}";

            if (opts.PropertiesToInclude.Any())
                path += $"{QueryParams.PROPERTIES}={opts.PropertiesToInclude}";

            if (opts.Offset.HasValue)
                path += $"{QueryParams.OFFSET}={opts.Offset}";

            var data = _client.Execute<CompanyListHubSpotModel<TCompany>, ListRequestOptions>(path, opts);
            return data;

        }

        /// <summary>
        /// Updates a given company entity, any changed properties are updated
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="entity">The company entity</param>
        /// <returns>The updated company entity</returns>
        public TCompany Update(TCompany entity)
        {
            if (entity.Id < 1)
                throw new ArgumentException("Company entity must have an id set!");

            return _client.Execute<TCompany, TCompany>(GetRoute<TCompany>("companies", entity.Id.ToString()), entity, Method.PUT);
        }

        /// <summary>
        /// Deletes the given company
        /// </summary>
        /// <param name="companyId">ID of the company</param>
        public void Delete(long companyId)
            => _client.ExecuteOnly(GetRoute<TCompany>("companies", companyId.ToString()), method: Method.DELETE);

        public CompanySearchHubSpotModel<TCompany> Search(SearchRequestOptions opts = null)
        {
            if (opts == null)
            {
                opts = new SearchRequestOptions();
            }

            var path = "/crm/v3/objects/companies/search";

            var data = _client.Execute<CompanySearchHubSpotModel<TCompany>, SearchRequestOptions>(path, opts, Method.POST);

            return data;
        }

        /// <summary>
        /// Gets a list of associations for a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="CompanyHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to get associations for</param>
        public TCompany GetAssociations(TCompany entity)
        {
            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var companyPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/6";
            long? offSet = null;

            var dealResults = new List<long>();
            do
            {
                var dealAssociations = _client.Execute<AssociationIdListHubSpotModel>(string.Format("{0}?limit=100{1}", companyPath, offSet == null ? null : "&offset=" + offSet));
                if (dealAssociations.Results.Any())
                    dealResults.AddRange(dealAssociations.Results);
                if (dealAssociations.HasMore)
                    offSet = dealAssociations.Offset;
                else
                    offSet = null;
            } while (offSet != null);
            if (dealResults.Any())
                entity.Associations.AssociatedDeals = dealResults.ToArray();
            else
                entity.Associations.AssociatedDeals = null;

            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var contactPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/2";

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