using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HubSpot.NET.Core.Requests
{
    public class RequestSerializer
    {
        private readonly RequestDataConverter _requestDataConverter;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSerializer"/> class.
        /// </summary>
        protected RequestSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestSerializer"/> class.
        /// </summary>
        /// <remarks>Use this constructor if you wish to override dependencies</remarks>
        /// <param name="requestDataConverter">The request data converter.</param>
        public RequestSerializer(RequestDataConverter requestDataConverter)
            : this()
        {
            _requestDataConverter = requestDataConverter;
        }

        /// <summary>
        /// Deserialize the given JSON into a <see cref="IHubSpotModel"/>
        /// </summary>
        /// <param name="json">The json data returned by HubSpot that should be converted</param>
        /// <param name="deserializeAsProperties">Does this entity use the properties schema (contacts, deals, companies)</param>
        /// <returns>The deserialized entity</returns>
        public virtual IHubSpotModel DeserializeEntity<T>(string json, bool deserializeAsProperties = true) where T : IHubSpotModel, new()
        {
            if (deserializeAsProperties)
            {
                var jobj = JsonConvert.DeserializeObject<ExpandoObject>(json);
                var converted = _requestDataConverter.FromHubSpotResponse<T>(jobj);

//                converted.FromHubSpotDataEntity(jobj);

                return converted;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
            }
        }

        /// <summary>
        /// Deserialize the given JSON from a List requet into a <see cref="IHubSpotModel"/>
        /// </summary>
        /// <param name="json">The JSON data returned from a List request to HubSpot</param>
        /// <param name="deserializeAsProperties">Does this entity use the properties schema (contacts, deals, companies)</param>
        /// <returns></returns>
        public virtual IHubSpotModel DeserializeListEntity<T>(string json, bool deserializeAsProperties = true) where T : IHubSpotModel, new()
        {
            if (deserializeAsProperties)
            {
                var expandoObject = JsonConvert.DeserializeObject<ExpandoObject>(json);
                var converted = _requestDataConverter.FromHubSpotListResponse<T>(expandoObject);
                return converted;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
            }
        }
    }
}
