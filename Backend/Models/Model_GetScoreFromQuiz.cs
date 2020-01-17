using Nest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_GetScoreFromQuiz
    {

        [JsonProperty(PropertyName = "max_score")]
        public int intMaxScore { get; set; }
    }
}
