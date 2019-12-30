using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class ObjectResultReturn
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "error_message")]
        public string strErrorMessage { get; set; }
    }
}
