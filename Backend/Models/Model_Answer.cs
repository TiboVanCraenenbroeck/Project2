using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_Answer
    {

        [JsonProperty(PropertyName = "answer_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "answer")]
        public string strAnswer { get; set; }
        
        [JsonProperty(PropertyName = "correct")]
        public bool blnCorrect { get; set; }
    }
}
