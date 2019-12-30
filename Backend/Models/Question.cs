using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Question
    {

        [JsonProperty(PropertyName = "question_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "question")]
        public string strQuestion { get; set; }
        
        [JsonProperty(PropertyName = "difficulty")]
        public int intDifficulty { get; set; }
        
        [JsonProperty(PropertyName = "answers")]
        public List<Answer> listAnswer { get; set; }
    }
}
