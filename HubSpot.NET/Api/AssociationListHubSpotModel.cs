using HubSpot.NET.Core.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    public class AssociationListHubSpotModel : IHubSpotModel
    {
        [DataMember(Name = "hasMore")]
        public bool HasMore { get; set; }

        [DataMember(Name = "offset")]
        public long? Offset { get; set; }

        [DataMember(Name = "toObjectId")]
        public long? ToObjectId { get; set; }

        [DataMember(Name = "results")]
        public List<AssociationResult> Results { get; set; }

        public bool IsNameValue => throw new System.NotImplementedException();

        public string RouteBasePath => throw new System.NotImplementedException();

        public void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
        }
    }
}
