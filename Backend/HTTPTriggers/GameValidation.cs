using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Backend.StaticFunctions;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.HTTPTriggers
{
    public static class GameValidation
    {
        [FunctionName("GameValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/gamevalidation/{QuizId}/{GameId}")] HttpRequest req, string QuizId, string GameId,
            ILogger log)
        {
            try
            {
                Guid guidGameId = Guid.Parse(GameId);
                ModelGameValidation NewModelGameValidation = new ModelGameValidation();
                // Check if the quiz exists
                if (await QuizExists.CheckIfQuizExistsAsync(Guid.Parse(QuizId)))
                {
                    // Check if the game_id exists
                    if (await SF_GameValidation.GameIdExistsAsync(guidGameId))
                    {
                        //Get the data from the body
                        string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                        ModelGameValidation IncomingModelGameValidation = JsonConvert.DeserializeObject<ModelGameValidation>(strJson);
                        //Check if it's the first game
                        if (IncomingModelGameValidation.intGameStatus == 0)
                        {
                            // Select a random question
                            //NewModelGameValidation.question = new Question();
                            //Question question = await SF_GameValidation.GetRandomQuestionAsync(guidGameId, IncomingModelGameValidation.question.intDifficulty);
                            NewModelGameValidation.question = await SF_GameValidation.GetRandomQuestionAsync(guidGameId, IncomingModelGameValidation.question.intDifficulty);
                            // Select a random team
                            List<Team> listTeams = await TeamFunctions.GetTeamFromGameAsync(guidGameId);
                            Random random = new Random();
                            int intRandom = random.Next(listTeams.Count);
                            NewModelGameValidation.team = listTeams[intRandom];
                        }
                        else
                        {
                            // Check if the answer is correct
                            // Check if it is the last question of the game
                            // Else --> Select a random team + next random question
                        }
                    }
                    else
                    {
                        NewModelGameValidation.strErrorMessage = "Deze game bestaat niet";
                    }
                }
                else
                {
                    NewModelGameValidation.strErrorMessage = "Deze quiz bestaat niet";
                }
                return new OkObjectResult(NewModelGameValidation);
            }
            catch (Exception ex)
            {
                log.LogError("GameValidation" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
