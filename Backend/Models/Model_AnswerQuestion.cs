using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_AnswerQuestion
    {
        [JsonProperty(PropertyName = "answer")]
        public string strRightAnswer { get; set; }

        [JsonProperty(PropertyName = "question")]
        public string strQuestion { get; set; }

    }
}
