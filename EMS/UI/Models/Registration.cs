using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class Registration
    {
        
        public string id { get; set; }

        public Guid regId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EventId { get; set; } // This references the EventDetails ID
        public bool IsDeleted { get; set; }

        public string RegStatus { get; set; }
        public Registration()
        {
            regId = Guid.NewGuid();
            id = regId.ToString();

            RegStatus = "pending";
        }
    }
}
