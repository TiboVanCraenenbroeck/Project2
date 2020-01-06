﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_GameValidation
    {
        [JsonProperty(PropertyName = "game_status")]
        public int intGameStatus { get; set; }
        
        [JsonProperty(PropertyName = "team")]
        public Model_Team team { get; set; }
        
        [JsonProperty(PropertyName = "question")]
        public Model_Question question { get; set; }
        
        [JsonProperty(PropertyName = "time")]
        public int intTime { get; set; }
        
        [JsonProperty(PropertyName = "number_of_correct_attempts")]
        public int intNumberOfCorrectAttempts { get; set; }
        
        [JsonProperty(PropertyName = "error_message")]
        public string strErrorMessage { get; set; }
    }
}
