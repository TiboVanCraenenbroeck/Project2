using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class LoginValidationReturn: User
    {

        [JsonProperty(PropertyName = "cookie_id")]
        public string Id { get; set; }
        
        [JsonProperty(PropertyName = "error_message")]
        public string strErrorMessage { get; set; }
    }
}
