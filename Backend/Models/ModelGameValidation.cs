using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class ModelGameValidation
    {
        [JsonProperty(PropertyName = "game_status")]
        public int intGameStatus { get; set; }
        
        [JsonProperty(PropertyName = "team")]
        public Team team { get; set; }
        
        [JsonProperty(PropertyName = "question")]
        public Question question { get; set; }
        
        [JsonProperty(PropertyName = "time")]
        public int intTime { get; set; }
        
        [JsonProperty(PropertyName = "number_of_correct_attempts")]
        public int intNumberOfCorrectAttempts { get; set; }
        
        [JsonProperty(PropertyName = "error_message")]
        public string strErrorMessage { get; set; }
    }
}
