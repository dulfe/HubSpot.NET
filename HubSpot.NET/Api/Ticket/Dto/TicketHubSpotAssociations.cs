using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HubSpot.NET.Api.Ticket.Dto
{
    public class TicketHubSpotAssociations
    {
        [DataMember(Name = "associatedCompanyIds")]
        public long[] AssociatedCompany { get; set; }

        [DataMember(Name = "associatedVids")]
        public long[] AssociatedContacts { get; set; }
    }
}
