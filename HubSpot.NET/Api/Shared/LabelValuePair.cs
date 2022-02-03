using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Shared
{
    [DataContract]
    public class LabelValuePair : IKeyValuePair
    {
        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        string IKeyValuePair.Key
        {
            get
            {
                return Label;
            }
            set
            {
                Label = value;
            }
        }

        string IKeyValuePair.Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }
    }
}
