﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_QuizSubject
    {

        [JsonProperty(PropertyName = "quiz_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "title")]
        public string strTitle { get; set; }
        
        [JsonProperty(PropertyName = "description")]
        public string strDescription { get; set; }
        
        [JsonProperty(PropertyName = "dateTime")]
        public DateTime dtDateTime { get; set; }
    }
}