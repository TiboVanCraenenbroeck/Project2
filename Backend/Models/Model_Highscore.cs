using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_Highscore
    {
        [JsonProperty(PropertyName = "team_id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string TeamName { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int score { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string image { get; set; }


    }
}
