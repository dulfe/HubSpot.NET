using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Ticket.Dto
{
    /// <summary>
    /// Models a set of results returned when querying for sets of tickets
    /// </summary>
    [DataContract]
    public class TicketListHubSpotModel<T> : IHubSpotModel where T : TicketHubSpotModel, new()
    {
        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }

        /// <summary>
        /// Gets or sets the tickets.
        /// </summary>
        /// <value>
        /// The tickets.
        /// </value>
        [DataMember(Name = "results")]
        public IList<T> Tickets { get; set; } = new List<T>();

        public string RouteBasePath => "/crm/v3/objects/tickets";

        public bool IsNameValue => false;
        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }
    }
}
