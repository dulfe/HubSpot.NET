using HubSpot.NET.Api;
using HubSpot.NET.Api.Deal.Dto;
using HubSpot.NET.Core;
using System;
using System.Collections.Generic;

namespace HubSpot.NET.Examples
{
    public class Deals
    {
        public static void Example(HubSpotApi api)
        {
            try
            {
                Tests(api);
                Console.WriteLine("Deals tests completed successfully!");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Deals tests failed!");
                Console.WriteLine(ex.ToString());
            }
        }
        private static void Tests(HubSpotApi api)
        {
            /**
             * Create a deal
             */
            var deal = api.Deal.Create(new DealHubSpotModel()
            {
                Amount = 10000,
                Name = "New Deal #1",
                DealType = "newbusiness",
                Stage = System.Configuration.ConfigurationManager.AppSettings["ClosedStageId"] ?? "closedlost",
                DateCreated = DateTime.UtcNow
            });

            /**
             * Update a deal
             */
            deal.Name = "Updated Deal #1";
            deal.CloseDate = DateTime.UtcNow;
            deal = api.Deal.Update(deal);

            /**
             * Search for a deal
             */
            var searchedDeal = api.Deal.Search(new SearchRequestOptions()
            {
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter
                            {
                                Operator = "EQ",
                                PropertyName = "dealname",
                                Value = deal.Name
                            }
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "dealname", "amount", "closedate", "pipeline", "dealstage"
                }
            });
            foreach (var d in searchedDeal.Results)
            {
                if (d.Id == null)
                    throw new InvalidOperationException("The found deal does not have an ID set");
                if (d.Name != deal.Name)
                    throw new InvalidOperationException("The found deal does not have the same name");
            }

            /**
             * Get Associations
             */
            deal = api.Deal.GetAssociations(deal);

            /**
             * Get all deals
             */
            var deals = api.Deal.List(false,
                new ListRequestOptions(250) { PropertiesToInclude = new List<string> { "dealname", "amount" } });

            /**
             *  Get all deals
             *  This is commented in case the HubSpot data has a large quantity of deals.
             */
            //var moreResults = true;
            //long offset = 0;
            //while (moreResults)
            //{
            //    var allDeals = api.Deal.List<DealHubSpotModel>(false,
            //        new ListRequestOptions { PropertiesToInclude = new List<string> { "dealname", "amount", "hubspot_owner_id", "closedate" }, Limit = 100, Offset = offset });

            //    moreResults = allDeals.MoreResultsAvailable;
            //    if (moreResults) offset = allDeals.ContinuationOffset;
            //}

            /**
             *  Get recently created deals since 7 days ago, limited to 10 records
             *  Will default to 30 day if Since is not set.
             *  Using DealRecentListHubSpotModel to accomodate deals returning in the "results" property.
             */
            var currentdatetime = DateTime.SpecifyKind(DateTime.Now.AddDays(-7), DateTimeKind.Utc);
            var since = ((DateTimeOffset)currentdatetime).ToUnixTimeMilliseconds().ToString();

            var recentlyCreatedDeals = api.Deal.RecentlyCreated(new DealRecentRequestOptions
            {
                Limit = 10,
                IncludePropertyVersion = false,
                Since = since
            });

            /**
             *  Get recently updated deals since 7 days ago, limited to 10 records
             *  Will default to 30 day if Since is not set.
             *  Using DealRecentListHubSpotModel to accomodate deals returning in the "results" property.
             */
            var recentlyUpdatedDeals = api.Deal.RecentlyUpdated(new DealRecentRequestOptions
            {
                Limit = 10,
                IncludePropertyVersion = false,
                Since = since
            });

            /**
             * Delete a deal
             */
            api.Deal.Delete(deal.Id.Value);
        }
    }
}
