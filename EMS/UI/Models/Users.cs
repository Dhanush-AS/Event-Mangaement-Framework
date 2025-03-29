using Newtonsoft.Json;
using System;

namespace UI.Models
{
    public class Users
    {
        [JsonProperty("id")]
        public string Id { get; set; }  
        public Guid Userid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role {  get; set; }
        public Users()
        {
            Userid = Guid.NewGuid();
            Id = Userid.ToString();
        }

    }

}
