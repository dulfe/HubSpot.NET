using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Attributes;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;

namespace HubSpot.NET.Api.Ticket.Dto
{
    [DataContract]
    public class TicketHubSpotModel : IHubSpotModel
    {
        public TicketHubSpotModel()
        {
            Associations = new TicketHubSpotAssociations();
        }

        /// <summary>
        /// Tickets unique Id in HubSpot
        /// </summary>        
        [JsonIgnore]
        public long? Id { get; set; }

        [DataMember(Name = "hs_ticket_id")]
        public long IdSetter
        {
            set { Id = value; }
        }
        
        [JsonIgnore]
        public long? ObjectId { get; set; }

        [DataMember(Name = "hs_object_id")]
        public long ObjectIdSetter
        {
            set { ObjectId = value; }
        }

        [DataMember(Name = "createdate")]
        [LongDate]
        public DateTime? DateCreated { get; set; }

        [DataMember(Name = "hs_lastmodifieddate")]
        [LongDate]
        public DateTime? DateModified { get; set; }

        [DataMember(Name = "hs_pipeline")]
        public string Pipeline { get; set; }

        [DataMember(Name = "hs_pipeline_stage")]
        public string Stage { get; set; }

        [DataMember(Name = "hs_ticket_category")]
        public string Category { get; set; }

        [DataMember(Name = "hs_ticket_priority")]
        public string Priority { get; set; }

        [DataMember(Name = "hubspot_owner_id")]
        public long? OwnerId { get; set; }
        
        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [DataMember(Name = "content")]
        public string Description { get; set; }

        [IgnoreDataMember]
        public TicketHubSpotAssociations Associations { get; }

        public bool IsNameValue => true;

        public string RouteBasePath => "/crm/v3/objects/tickets";

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }

        public virtual void ToHubSpotDataEntity(ref dynamic dataEntity)
        {
            dataEntity.hs_ticket_id = null;
            dataEntity.hs_object_id = null;
        }
    }
}
