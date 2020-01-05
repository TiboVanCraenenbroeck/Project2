using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_CreateNewUserAccountReturn
    {

        [JsonProperty(PropertyName = "succeeded")]
        public bool blSucceeded { get; set; }
        
        [JsonProperty(PropertyName = "message")]
        public string strMessage { get; set; }
    }
}
