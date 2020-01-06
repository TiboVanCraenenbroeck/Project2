using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Models
{
    public class Model_Game
    {

        [JsonProperty(PropertyName = "game_id")]
        public Guid Id { get; set; }
        
        [JsonProperty(PropertyName = "quiz")]
        public Model_QuizSubject quizSubject { get; set; }
        
        [JsonProperty(PropertyName = "teams")]
        public List<Model_Team> teams { get; set; }

        [JsonProperty(PropertyName = "dateTime")]
        public DateTime dtDateTime { get; set; }
    }
}
