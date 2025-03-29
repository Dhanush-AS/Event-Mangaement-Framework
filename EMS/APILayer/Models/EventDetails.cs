using Newtonsoft.Json;
using System;

namespace EMS2.Models
{
    public class EventDetails
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        //generates guid
        public Guid eventId { get; set; }

        public string EventName { get; set; }

        public string EventCategory { get; set; }

        public DateTime EventDate { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }
        public bool IsDeleted {  get; set; }

        public EventDetails()
        {
            eventId = Guid.NewGuid();
            Id = eventId.ToString(); 
        }
    }
}
