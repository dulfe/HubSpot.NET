using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HubSpot.NET.Api.Shared
{
    [DataContract]
    public class NameValuePair : IKeyValuePair
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        string IKeyValuePair.Key
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
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
