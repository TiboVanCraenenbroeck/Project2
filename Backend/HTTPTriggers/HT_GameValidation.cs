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
    public static class HT_GameValidation
    {
        [FunctionName("GameValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/gamevalidation/{QuizId}/{GameId}")] HttpRequest req, string QuizId, string GameId,
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
                        //Get the data from the body
                        string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                        Model_GameValidation IncomingModelGameValidation = JsonConvert.DeserializeObject<Model_GameValidation>(strJson);
                        //Check if it's the first game
                        if (IncomingModelGameValidation.intGameStatus == 0)
                        {
                            // Select a random question
                            NewModelGameValidation.question = await SF_GameValidation.GetRandomQuestionAsync(guidGameId, IncomingModelGameValidation.question.intDifficulty);
                            // Select a random team
                            List<Model_Team> listTeams = await SF_TeamFunctions.GetTeamFromGameAsync(guidGameId);
                            Random random = new Random();
                            int intRandom = random.Next(listTeams.Count);
                            NewModelGameValidation.team = listTeams[intRandom];
                        }
                        else
                        {
                            // Check if the answer is correct
                            Model_GameValidation validateModelGameValidation = await SF_GameValidation.CheckAnswerAsync(IncomingModelGameValidation, guidGameId);
                            if (validateModelGameValidation.team.intScore == 0)
                            {
                                // Select other team
                                List<Model_Team> listTeams = await SF_TeamFunctions.GetTeamFromGameAsync(guidGameId);
                                foreach (Model_Team itemTeam in listTeams)
                                {
                                    if (itemTeam.Id != validateModelGameValidation.team.Id)
                                    {
                                        NewModelGameValidation.team = itemTeam;
                                        break;
                                    }
                                }
                                NewModelGameValidation.intNumberOfCorrectAttempts = 0;
                            }
                            else
                            {
                                NewModelGameValidation.team = IncomingModelGameValidation.team;
                                NewModelGameValidation.intNumberOfCorrectAttempts = validateModelGameValidation.intNumberOfCorrectAttempts;
                            }
                            // Select a random question
                            NewModelGameValidation.question = await SF_GameValidation.GetRandomQuestionAsync(guidGameId, IncomingModelGameValidation.question.intDifficulty);
                            // Check if there is a question
                            if(NewModelGameValidation.question.Id.ToString()== "00000000-0000-0000-0000-000000000000")
                            {
                                NewModelGameValidation.intGameStatus = 2;
                            }
                            else
                            {
                                NewModelGameValidation.intGameStatus = 1;
                            }
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
