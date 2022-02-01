using System;
using System.Collections.Generic;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using System;

namespace HubSpot.NET.Examples
{
    public class Companies
    {
        public static void Example(HubSpotApi api)
        {
            try
            {
                Tests(api);
                Console.WriteLine("Companies example completed successfully.");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Companies tests failed!");
                Console.WriteLine(ex.ToString());
            }
           
        }

        private static void Tests(HubSpotApi api)
        {
            /**
             * Create a company
             */
            var company = api.Company.Create(new CompanyHubSpotModel()
            {
                Domain = "squaredup.com",
                Name = "Squared Up"
            });

            /**
             * Update a company's property
             */
            company.Description = "Data Visualization for Enterprise IT";
            api.Company.Update(company);


            /**
             * Get all companies with domain name "squaredup.com"
             */
            var companies = api.Company.GetByDomain("squaredup.com", new CompanySearchByDomain()
            {
                Limit = 10
            });

            /**
             * Search for a company
             */
            var searchedCompany = api.Company.Search<CompanyHubSpotModel>(new SearchRequestOptions()
            {
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter
                            {
                                PropertyName = "name",
                                Value = company.Name
                            }
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "domain", "name", "website"
                }
            });
            if (searchedCompany.Total < 1)
                throw new InvalidOperationException("No companies found.");
            foreach (var d in searchedCompany.Results)
            {
                if (d.Id == null)
                    throw new InvalidOperationException("The found company does not have an ID set");
                if (d.Name != company.Name)
                    throw new InvalidOperationException("The found company does not have the same name");
            }

            /**
             * Get Associations
             */
            company = api.Company.GetAssociations(company);
           
            /**
             * Delete a contact
             */
            api.Company.Delete(company.Id.Value);
        }
    }
}
