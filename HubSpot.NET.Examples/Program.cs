using System;
#if NET461
using System.Configuration;
#else
using Microsoft.Extensions.Configuration;
#endif

using HubSpot.NET.Core;
using HubSpot.NET.Core.OAuth.Dto;

namespace HubSpot.NET.Examples
{
    public class Examples
    {
#if NET461
#else
        internal static IConfigurationRoot Configuration { get; private set; }
#endif

        static void Main(string[] args)
        {
#if NET461
#else
            // C# ConfigurationBuilder example for Azure Functions v2 runtime
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
#endif

            /**
             * Initialize the API with your API Key
             * You can find or generate this under Integrations -> HubSpot API key
             */
            string privateAppAccessToken = GetAppSetting("PrivateAppAccessToken");
            if (string.IsNullOrWhiteSpace(privateAppAccessToken))
            {
                Console.WriteLine("Create a settings file (local.settings.json for .NET Core, app.config for .NET) and add a Private App Access Token as 'PrivateAppAccessToken' to run tests.");
                return;
            }

            var token = new HubSpotToken { AccessToken = privateAppAccessToken };
            var api = new HubSpotApi(token);

            Deals.Example(api);

            Companies.Example(api);

            Contacts.Example(api);

            CompanyProperties.Example(api);

            EmailSubscriptions.Example(api);
        }

        public static string GetAppSetting(string key)
        {
#if NET461
            return ConfigurationManager.AppSettings[key];
#else
            return Configuration.GetSection($"Values:{key}").Value;
#endif
        }
    }
}
