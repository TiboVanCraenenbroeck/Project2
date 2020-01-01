using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Team
    {

        [JsonProperty(PropertyName = "team_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string strName { get; set; }
        
        [JsonProperty(PropertyName = "avatar")]
        public Avatar avatar { get; set; }
    }
}
