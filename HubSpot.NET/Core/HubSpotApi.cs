namespace HubSpot.NET.Core
{
    using HubSpot.NET.Api.Company;
    using HubSpot.NET.Api.Contact;
    using HubSpot.NET.Api.Deal;
    using HubSpot.NET.Api.EmailEvents;
    using HubSpot.NET.Api.EmailSubscriptions;
    using HubSpot.NET.Api.Engagement;
    using HubSpot.NET.Api.Files;
    using HubSpot.NET.Api.OAuth;
    using HubSpot.NET.Api.OAuth.Dto;
    using HubSpot.NET.Api.Owner;
    using HubSpot.NET.Api.Pipeline;
    using HubSpot.NET.Api.Properties;
    using HubSpot.NET.Api.Timeline;
    using HubSpot.NET.Core.Interfaces;

    /// <summary>
    /// Starting point for using HubSpot.NET
    /// </summary>
    public class HubSpotApi
        : HubSpotApi<Api.Company.Dto.CompanyHubSpotModel, Api.Contact.Dto.ContactHubSpotModel, Api.Deal.Dto.DealHubSpotModel>
        , IHubSpotApi
    {
        /// <summary>
        /// Creates a HubSpotApi using API key authentication instead of OAuth
        /// </summary>
        /// <param name="apiKey">The HubSpot API key for your application.</param>
        /// <summary>
        /// Creates a HubSpotApi using API key authentication instead of OAuth
        /// </summary>
        /// <param name="apiKey">The HubSpot API key for your application.</param>
        public HubSpotApi(string apiKey)
            : base(apiKey)
        {
        }

        /// <summary>
        /// Creates a HubSpotApi using OAuth 2.0 authentication for all API calls. 
        /// </summary>
        public HubSpotApi(string clientId, string clientSecret, string appId, HubSpotToken token = null)
            : base(clientId, clientSecret, appId, token)
        {
        }

        protected override void InitializeCustomizableRepos(IHubSpotClient client, string clientId = "", string clientSecret = "")
        {
            Company = new HubSpotCompanyApi(client);
            Contact = new HubSpotContactApi(client);
            Deal = new HubSpotDealApi(client);
        }
    }

    /// <summary>
    /// Starting point for using HubSpot.NET
    /// </summary>
    public class HubSpotApi<TCompanyHubSpotModel, TContactHubSpotModel, TDealHubSpotModel>
        : IHubSpotApi<TCompanyHubSpotModel, TContactHubSpotModel, TDealHubSpotModel>
        where TCompanyHubSpotModel : Api.Company.Dto.CompanyHubSpotModel, new()
        where TContactHubSpotModel : Api.Contact.Dto.ContactHubSpotModel, new()
        where TDealHubSpotModel : Api.Deal.Dto.DealHubSpotModel, new()
    {
        public IHubSpotOAuthApi OAuth { get; set; }
        public IHubSpotCompanyApi<TCompanyHubSpotModel> Company { get; protected set; }
        public IHubSpotContactApi<TContactHubSpotModel> Contact { get; protected set; }
        public IHubSpotContactPropertyApi ContactProperty { get; protected set; }
        public IHubSpotDealApi<TDealHubSpotModel> Deal { get; protected set; }
        public IHubSpotEngagementApi Engagement { get; protected set; }
        public IHubSpotCosFileApi File { get; protected set; }
        public IHubSpotOwnerApi Owner { get; protected set; }
        public IHubSpotCompanyPropertiesApi CompanyProperties { get; protected set; }
        public IHubSpotCompanyPropertyGroupsApi CompanyPropertyGroups { get; protected set; }
        public IHubSpotEmailEventsApi EmailEvents { get; protected set; }
        public IHubSpotEmailSubscriptionsApi EmailSubscriptions { get; protected set; }
        public IHubSpotTimelineApi Timelines { get; protected set; }
        public IHubSpotPipelineApi Pipelines { get; protected set; }

        /// <summary>
        /// Creates a HubSpotApi using API key authentication instead of OAuth
        /// </summary>
        /// <param name="apiKey">The HubSpot API key for your application.</param>
        /// <summary>
        /// Creates a HubSpotApi using API key authentication instead of OAuth
        /// </summary>
        /// <param name="apiKey">The HubSpot API key for your application.</param>
        public HubSpotApi(string apiKey)
        {
            IHubSpotClient client = new HubSpotBaseClient(apiKey, HubSpotAuthenticationMode.HAPIKEY);
            InitializeRepos(client);
        }

        /// <summary>
        /// Creates a HubSpotApi using OAuth 2.0 authentication for all API calls. 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="appId"></param>
        /// <param name="token"></param>
        public HubSpotApi(string clientId, string clientSecret, string appId, HubSpotToken token = null)
        {
            IHubSpotClient client = new HubSpotBaseClient(string.Empty, HubSpotAuthenticationMode.OAUTH, appId, token);
            InitializeRepos(client, clientId, clientSecret);
        }

        protected virtual void InitializeRepos(IHubSpotClient client, string clientId = "", string clientSecret = "")
        {
            OAuth = new HubSpotOAuthApi(client, clientId, clientSecret);
            Engagement = new HubSpotEngagementApi(client);
            File = new HubSpotCosFileApi(client);
            Owner = new HubSpotOwnerApi(client);
            CompanyProperties = new HubSpotCompaniesPropertiesApi(client);
            CompanyPropertyGroups = new HubSpotCompanyPropertyGroupsApi(client);
            Timelines = new HubSpotTimelineApi(client);
            Pipelines = new HubSpotPipelinesApi(client);
            EmailEvents = new HubSpotEmailEventsApi(client);
            EmailSubscriptions = new HubSpotEmailSubscriptionsApi(client);
            ContactProperty = new HubSpotContactPropertyApi(client);

            InitializeCustomizableRepos(client, clientId, clientSecret);
        }

        protected virtual void InitializeCustomizableRepos(IHubSpotClient client, string clientId = "", string clientSecret = "")
        {
            Company = new HubSpotCompanyApi<TCompanyHubSpotModel>(client);
            Contact = new HubSpotContactApi<TContactHubSpotModel>(client);
            Deal = new HubSpotDealApi<TDealHubSpotModel>(client);
        }
    }
}
