using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_User
    {

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "surname")]
        public string strSurname { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string strName { get; set; }
        
        [JsonProperty(PropertyName = "mail")]
        public string strMail { get; set; }
        
        [JsonProperty(PropertyName = "password")]
        public string strPassword { get; set; }
    }
}
