using System.Collections.Generic;
using System.Threading.Tasks;
using HubSpot.NET.Core.OAuth.Dto;
using RestSharp;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotClientAsync
    {
        Task<T> ExecuteAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();

        Task<T> ExecuteAsync<T>(string absoluteUriPath, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();

        Task ExecuteAsync(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true);

        Task ExecuteBatchAsync(string absoluteUriPath, List<object> entities, Method method = Method.GET, bool convertToPropertiesSchema = true);

        Task<T> ExecuteMultipartAsync<T>(string absoluteUriPath, byte[] data, string filename, Dictionary<string,string> parameters, Method method = Method.POST) where T : new();

        Task<T> ExecuteListAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new();
    }
}