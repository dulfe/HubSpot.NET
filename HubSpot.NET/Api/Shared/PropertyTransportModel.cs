namespace HubSpot.NET.Api.Shared
{
    using HubSpot.NET.Api.Contact.Dto;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;

    [DataContract]
    public abstract class PropertyTransport<T>
    {
        [DataMember(Name = "properties")]
        public PropertyValuePairCollection Properties { get; set; } = new PropertyValuePairCollection();
        
        public void ToPropertyTransportModel(T model)
        {
            new KeyValuePairHelper().ToPropertyTransportModel(model, Properties);
        }

        public void FromPropertyTransportModel(out T model)
        {
            model = new KeyValuePairHelper().FromPropertyTransportModel<T, PropertyValuePair>(Properties);
        }
    }
}
