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
using System.Data.SqlClient;

namespace Backend.HTTPTriggers
{
    public static class NewGame
    {
        [FunctionName("NewGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/game/{QuizId}")] HttpRequest req, string QuizId,
        ILogger log)
        {
            /*
             {
    "game_id": "00000000-0000-0000-0000-000000000000",
    "quiz": {
        "quiz_id": "00000000-0000-0000-0000-000000000000",
        "title": null,
        "description": null,
        "dateTime": "0001-01-01T00:00:00"
    },
    "teams": [
        {
            "team_id": "00000000-0000-0000-0000-000000000000",
            "name": null,
            "avatar": {
                "avatar_id": "00000000-0000-0000-0000-000000000000",
                "name": null,
                "link": null
            }
        }
    ],
    "dateTime": "0001-01-01T00:00:00"
}
             */
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                ObjectResultReturn objectResultReturn = new ObjectResultReturn();
                Guid guidQuizId = Guid.Parse(QuizId);

                // Check if the user is logged in
                if (await IsUserLoggedIn.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Check if the quiz exist
                    if (await QuizExists.CheckIfQuizExistsAsync(guidQuizId))
                    {
                        //Ophalen van de data
                        string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                        Game newGame = JsonConvert.DeserializeObject<Game>(strJson);
                        newGame.Id = Guid.NewGuid();


                    }
                    else
                    {
                        objectResultReturn.Id = "ERROR";
                        objectResultReturn.strErrorMessage = "Dit onderwerp bestaat niet";
                    }
                }
                else
                {
                    // User is not logged in
                    objectResultReturn.Id = "ERROR";
                    objectResultReturn.strErrorMessage = "U bent afgemeld";
                }
                return new OkObjectResult(objectResultReturn);
            }
            catch (Exception ex)
            {
                log.LogError("NewGame" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
