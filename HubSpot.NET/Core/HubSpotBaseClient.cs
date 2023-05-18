using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.OAuth.Dto;
using HubSpot.NET.Core.Requests;
using HubSpot.NET.Core.Serializers;
using RestSharp;

namespace HubSpot.NET.Core
{
    public class HubSpotBaseClient : IHubSpotClient
#if NET452_OR_GREATER
        , IHubSpotClientAsync
#endif
    {
        protected readonly RequestSerializer _serializer = new RequestSerializer(new RequestDataConverter());
        private RestClient _client;

        public static string BaseUrl { get => "https://api.hubapi.com"; }

        protected readonly HubSpotAuthenticationMode _mode;

        // Used for HAPIKEY method
        protected readonly string _apiKeyName = "hapikey";
        protected readonly string _apiKey;

        // Used for OAUTH
        private HubSpotToken _token;

        protected virtual void Initialise()
        {
            _client = new RestClient(BaseUrl);
        }

        /// <summary>
        /// Creates a HubSpot client with the authentication scheme HAPIKEY.
        /// </summary>
        public HubSpotBaseClient(string apiKey)
        {
            _apiKey = apiKey;
            _mode = HubSpotAuthenticationMode.HAPIKEY;
            Initialise();
        }

        /// <summary>
        /// Creates a HubSpot client with the authentication scheme OAUTH.
        /// </summary>
        public HubSpotBaseClient(HubSpotToken token)
        {
            _token = token;
            _mode = HubSpotAuthenticationMode.OAUTH;
            Initialise();
        }

        public T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            string json = (method == Method.GET || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, convertToPropertiesSchema);

            T data = SendRequest(absoluteUriPath, method, json, responseData => (T)_serializer.DeserializeEntity<T>(responseData, convertToPropertiesSchema));

            return data;
        }

        public T Execute<T>(string absoluteUriPath, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            T data = SendRequest(absoluteUriPath, method, null, responseData => (T)_serializer.DeserializeEntity<T>(responseData, convertToPropertiesSchema));

            return data;
        }

        public void Execute(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true)
        {
            string json = (method == Method.GET || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, convertToPropertiesSchema);

            SendRequest(absoluteUriPath, method, json);
        }

        public void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.GET,
            bool convertToPropertiesSchema = true)
        {
            string json = (method == Method.GET || entities == null)
                ? null
                : _serializer.SerializeEntity(entities, convertToPropertiesSchema);

            SendRequest(absoluteUriPath, method, json);
        }

        public T ExecuteMultipart<T>(string absoluteUriPath, byte[] data, string filename, Dictionary<string, string> parameters, Method method = Method.POST) where T : new()
        {
            string path = $"{BaseUrl}{absoluteUriPath}";
            IRestRequest request = ConfigureRequestAuthentication(path, method);

            request.AddFile(filename, data, filename);

            foreach (KeyValuePair<string, string> kvp in parameters)
                request.AddParameter(kvp.Key, kvp.Value);

            IRestResponse<T> response = _client.Execute<T>(request);

            T responseData = response.Data;

            if (!response.IsSuccessful())
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription));

            return responseData;
        }

        public T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            string json = (method == Method.GET || entity == null)
                ? null
                : _serializer.SerializeEntity(entity);

            var data = SendRequest(
                absoluteUriPath,
                method,
                json,
                responseData => (T)_serializer.DeserializeListEntity<T>(responseData, convertToPropertiesSchema));
            return data;
        }

        protected virtual T SendRequest<T>(string path, Method method, string json, Func<string, T> deserializeFunc) where T : IHubSpotModel, new()
        {
            string responseData = SendRequest(path, method, json);

            if (string.IsNullOrWhiteSpace(responseData))
                return default;

            return deserializeFunc(responseData);
        }

        protected virtual string SendRequest(string path, Method method, string json)
        {
            IRestRequest request = ConfigureRequestAuthentication(path, method);

            if (method != Method.GET && !string.IsNullOrWhiteSpace(json))
                request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = _client.Execute(request);

            string responseData = response.Content;

            if (!response.IsSuccessful())
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription), responseData);

            return responseData;
        }

        /// <summary>
        /// Configures a <see cref="RestRequest"/> based on the authentication scheme detected and configures the endpoint path relative to the base path.
        /// </summary>
        protected virtual RestRequest ConfigureRequestAuthentication(string path, Method method)
        {
#if NET451
            RestRequest request = new RestRequest(path, method);
            request.RequestFormat = DataFormat.Json;
#else
            RestRequest request = new RestRequest(path, method, DataFormat.Json);
#endif
            switch (_mode)
            {
                case HubSpotAuthenticationMode.OAUTH:
                    request.AddHeader("Authorization", GetAuthHeader(_token));
                    break;
                default:
                    request.AddQueryParameter(_apiKeyName, _apiKey);
                    break;
            }

            request.JsonSerializer = new NewtonsoftRestSharpSerializer();
            return request;
        }

        protected virtual string GetAuthHeader(HubSpotToken token) => $"Bearer {token.AccessToken}";


        /// <summary>
        /// Updates the OAuth token used by the client.
        /// </summary>
        public void UpdateToken(HubSpotToken token) => _token = token;

#if NET452_OR_GREATER
        #region Async Support
        public async Task<T> ExecuteAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            string json = (method == Method.GET || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, convertToPropertiesSchema);

            T data = await SendRequestAsync(absoluteUriPath, method, json, responseData => (T)_serializer.DeserializeEntity<T>(responseData, convertToPropertiesSchema))
                .ConfigureAwait(false);

            return data;
        }

        public async Task<T> ExecuteAsync<T>(string absoluteUriPath, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            T data = await SendRequestAsync(absoluteUriPath, method, null, responseData => (T)_serializer.DeserializeEntity<T>(responseData, convertToPropertiesSchema))
                .ConfigureAwait(false);

            return data;
        }

        public async Task ExecuteAsync(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true)
        {
            string json = (method == Method.GET || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, convertToPropertiesSchema);

            await SendRequestAsync(absoluteUriPath, method, json).ConfigureAwait(false);
        }

        public async Task ExecuteBatchAsync(string absoluteUriPath, List<object> entities, Method method = Method.GET,
            bool convertToPropertiesSchema = true)
        {
            string json = (method == Method.GET || entities == null)
                ? null
                : _serializer.SerializeEntity(entities, convertToPropertiesSchema);

            await SendRequestAsync(absoluteUriPath, method, json).ConfigureAwait(false);
        }

        public async Task<T> ExecuteMultipartAsync<T>(string absoluteUriPath, byte[] data, string filename, Dictionary<string, string> parameters, Method method = Method.POST) where T : new()
        {
            string path = $"{BaseUrl}{absoluteUriPath}";
            IRestRequest request = ConfigureRequestAuthentication(path, method);

            request.AddFile(filename, data, filename);

            foreach (KeyValuePair<string, string> kvp in parameters)
                request.AddParameter(kvp.Key, kvp.Value);

            IRestResponse<T> response = await _client.ExecuteAsync<T>(request).ConfigureAwait(false);

            T responseData = response.Data;

            if (!response.IsSuccessful())
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription));

            return responseData;
        }

        public async Task<T> ExecuteListAsync<T>(string absoluteUriPath, object entity = null, Method method = Method.GET, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            string json = (method == Method.GET || entity == null)
                ? null
                : _serializer.SerializeEntity(entity);

            var data = await SendRequestAsync(
                    absoluteUriPath,
                    method,
                    json,
                    responseData => (T)_serializer.DeserializeListEntity<T>(responseData, convertToPropertiesSchema))
                .ConfigureAwait(false);
            return data;
        }

        protected virtual async Task<T> SendRequestAsync<T>(string path, Method method, string json, Func<string, T> deserializeFunc) where T : IHubSpotModel, new()
        {
            string responseData = await SendRequestAsync(path, method, json).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(responseData))
                return default;

            return deserializeFunc(responseData);
        }

        protected virtual async Task<string> SendRequestAsync(string path, Method method, string json)
        {
            IRestRequest request = ConfigureRequestAuthentication(path, method);

            if (method != Method.GET && !string.IsNullOrWhiteSpace(json))
                request.AddParameter("application/json", json, ParameterType.RequestBody);

            IRestResponse response = await _client.ExecuteAsync(request).ConfigureAwait(false);

            string responseData = response.Content;

            if (!response.IsSuccessful())
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription), responseData);

            return responseData;
        }
        #endregion
#endif
    }
}