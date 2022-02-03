namespace HubSpot.NET.Api.Shared
{
	using HubSpot.NET.Core;
	using HubSpot.NET.Core.Attributes;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
    using System.Runtime.Serialization;

    [DataContract]
    public class NameTransportModel<T>
    {
        [DataMember(Name = "properties")]
        public List<NameValuePair> Properties { get; set; } = new List<NameValuePair>();


        public void ToPropertyTransportModel(T model)
        {
            new KeyValuePairHelper().ToPropertyTransportModel(model, Properties);
        }

        public void FromPropertyTransportModel(out T model)
        {
            model = new KeyValuePairHelper().FromPropertyTransportModel<T, NameValuePair>(Properties);
        }
    }
}
