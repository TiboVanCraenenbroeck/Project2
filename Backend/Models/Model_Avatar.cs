using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_Avatar
    {

        [JsonProperty(PropertyName = "avatar_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string strName { get; set; }
        
        [JsonProperty(PropertyName = "moving_avatar")]
        public string strMovingAvatar { get; set; }
        
        [JsonProperty(PropertyName = "static_avatar_big")]
        public string strStaticAvatarBig { get; set; }
        
        [JsonProperty(PropertyName = "static_avatar_small")]
        public string strStaticAvatarSmall { get; set; }
    }
}
