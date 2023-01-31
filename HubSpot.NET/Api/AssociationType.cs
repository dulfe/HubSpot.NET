using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    public class AssociationType
    {
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "typeId")]
        public long TypeId { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
    }
}
