using Newtonsoft.Json;
using System;
namespace UI.Models
{
    public class EventDetails
    {
        public string id { get; set; }
        public Guid eventId { get; set; }
        public string EventName { get; set; }

        public string EventCategory { get; set; }

        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public EventDetails()
        {
            // Auto-generate the 'EventId' GUID and set 'Id' as string GUID
            eventId = Guid.NewGuid();
            id = eventId.ToString(); // Assign GUID string to Cosmos 'id'
        }
    }
}