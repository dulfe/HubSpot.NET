namespace HubSpot.NET.Examples
{
using HubSpot.NET.Core;
    using System;
    using System.IO;

    public class Examples
    {
        // enable args to be presented from CLI for automated test execution 
        static void Main(string[] args)
        {
            /**
             * Initialize the API with your API Key
             * You can find or generate this under Integrations -> HubSpot API key
             */
            string hapiKey = System.Configuration.ConfigurationManager.AppSettings["ApiKey"]; // YOUR KEY GOES HERE ... or supply it as args[0] either way.
            string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"]; // args[1]
            string clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"]; // args[2]
            string appId = System.Configuration.ConfigurationManager.AppSettings["AppId"]; // args[3]

            if(args.Length >= 1)
            {
                hapiKey = args[0];

                if(args.Length > 3)
                {
                    clientId = args[1];
                    clientSecret = args[2];
                    appId = args[3];
                }
            }
            /*
            else
            {
                bool authChosen = false;
                Console.WriteLine("How would you like to authenticate your requests? HAPIKey or OAuth?");
                string authType;
                while (authChosen == false)
                {
                    authType = Console.ReadLine().ToLowerInvariant();

                    switch (authType)
                    {
                        case "hapikey":
                            Console.WriteLine("Please enter the HAPIKey:");
                            bool valid = Guid.TryParse(Console.ReadLine(), out Guid guidResult);
                            hapiKey = valid ? guidResult.ToString() : string.Empty;

                            if (string.IsNullOrWhiteSpace(hapiKey))
                            {
                                Console.WriteLine("That is not a valid HAPIKey. Please try again");
                                break;
                            }

                            authChosen = true;

                            break;
                        case "oauth":
                            Console.WriteLine("Please enter the ClientID for your app:");
                            clientId = Console.ReadLine();

                            Console.WriteLine("Please enter the ClientSecret for your app:");
                            clientSecret = Console.ReadLine();

                            Console.WriteLine("Please enter the AppId for your app:");
                            appId = Console.ReadLine();

                            authChosen = true;
                            break;
                        default:
                            Console.WriteLine("That is not a valid selection. Please choose HAPIKey or OAuth.");
                            break;
                    }

                }
            }
            */


            if (string.IsNullOrWhiteSpace(hapiKey))
            {
                Console.WriteLine("Invalid apiKey. Skipping API related tests...");
            }
            else
            {
                var hapiApi = new HubSpotApi(hapiKey);
                RunApiKeyExamples(hapiApi);
            }

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(appId))
            {
                Console.WriteLine("Invalid clientId, Secret, or AppID. Skipping OAuth related tests...");
            }
            else
            {
                var oauthApi = new HubSpotApi(clientId, clientSecret, appId);
                RunOAuthExamples(oauthApi);
            }
        }

        private static void RunApiKeyExamples(HubSpotApi hapiApi)
        {
            EmailSubscriptions.Example(hapiApi);
            Deals.Example(hapiApi);
            Contacts.Example(hapiApi);
            Companies.Example(hapiApi);
            CompanyProperties.Example(hapiApi);
            Pipelines.Example(hapiApi);
        }

        private static void RunOAuthExamples(HubSpotApi oauthApi)
        {
            OAuth.Example(oauthApi);
            Timeline.Example(oauthApi);
        }

        private static string GetContactString() 
            => File.ReadAllText("ContactExample.txt");
    }
}
