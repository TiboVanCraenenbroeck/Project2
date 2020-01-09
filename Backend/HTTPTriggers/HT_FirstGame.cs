using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Backend.Models;
using Backend.StaticFunctions;
using System.Collections.Generic;

namespace Backend.HTTPTriggers
{
    public static class HT_FirstGame
    {
        [FunctionName("HT_FirstGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/gamevalidation/{QuizId}/{GameId}")] HttpRequest req, string QuizId, string GameId,
            ILogger log)
        {
            try
            {
                Guid guidGameId = Guid.Parse(GameId);
                Model_GameValidation NewModelGameValidation = new Model_GameValidation();
                // Check if the quiz exists
                if (await SF_QuizExists.CheckIfQuizExistsAsync(Guid.Parse(QuizId)))
                {
                    // Check if the game_id exists
                    if (await SF_GameValidation.GameIdExistsAsync(guidGameId))
                    {
                        // Select a random question
                        NewModelGameValidation.question = await SF_GameValidation.GetRandomQuestionAsync(guidGameId, 0);
                        // Check if their are questions in the quiz
                        if (NewModelGameValidation.question.Id.ToString() == "00000000-0000-0000-0000-000000000000")
                        {
                            NewModelGameValidation.strErrorMessage = "Er zitten geen vragen in dit onderwerp";
                            NewModelGameValidation.intGameStatus = 2;
                        }
                        else
                        {
                            // Select a random team
                            List<Model_Team> listTeams = await SF_TeamFunctions.GetTeamFromGameAsync(guidGameId);
                            Random random = new Random();
                            int intRandom = random.Next(listTeams.Count);
                            NewModelGameValidation.team = listTeams[intRandom];
                            NewModelGameValidation.intGameStatus = 1;
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
                log.LogError("HT_FirstGame" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
