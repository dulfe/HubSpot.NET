#if NET472
using System.Configuration;
#else
using Microsoft.Extensions.Configuration;
#endif

using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.OAuth.Dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	internal class TestSetUp
	{
		internal static IHubSpotClient Client { get; private set; } 

#if NET472
#else
		internal static IConfigurationRoot Configuration { get; private set; }
#endif

		[AssemblyInitialize]
		public static void Startup(TestContext testContext)
		{

#if NET472
#else
			// C# ConfigurationBuilder example for Azure Functions v2 runtime
			Configuration = new ConfigurationBuilder()
				.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.Build();
#endif

			string privateAppAccessToken = GetAppSetting("PrivateAppAccessToken");
			if (!string.IsNullOrWhiteSpace(privateAppAccessToken))
				Client = new HubSpotBaseClient(new HubSpotToken { AccessToken = privateAppAccessToken });
			else
				Assert.Fail("Create a settings file (local.settings.json for .NET Core, app.config for .NET) and add a Private App Access Token as 'PrivateAppAccessToken' to run tests.");
		}

		public static string GetAppSetting(string key)
		{
#if NET472
			return ConfigurationManager.AppSettings[key];
#else
			return Configuration.GetSection($"Values:{key}").Value;
#endif
		}
	}
}