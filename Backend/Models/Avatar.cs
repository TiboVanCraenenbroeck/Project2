﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Avatar
    {

        [JsonProperty(PropertyName = "avatar_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string strName { get; set; }
        
        [JsonProperty(PropertyName = "link")]
        public string strLink { get; set; }
    }
}