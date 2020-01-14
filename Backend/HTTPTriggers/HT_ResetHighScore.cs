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

namespace Backend.HTTPTriggers
{
    public static class HT_ResetHighScore
    {
        [FunctionName("ResetHighScore")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/highscores/{QuizId}")] HttpRequest req, string QuizId,
            ILogger log)
        {
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                Guid guidQuizId = Guid.Parse(QuizId);
                Model_ObjectResultReturn ObjectResultReturn = new Model_ObjectResultReturn();
                // Check if the user is logged in
                if (await SF_IsUserLoggedIn.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Check if the Quiz exists
                    if (await SF_QuizExists.CheckIfQuizExistsAsync(guidQuizId))
                    {
                        // Delete all the games with this QuizId
                    }
                    else
                    {
                        ObjectResultReturn.Id = "ERROR";
                        ObjectResultReturn.strErrorMessage = "Dit onderwerp bestaat niet";
                    }
                }
                else
                {
                    // User is not logged in
                    ObjectResultReturn.Id = "ERROR";
                    ObjectResultReturn.strErrorMessage = "U bent afgemeld";
                }
                return new OkObjectResult(ObjectResultReturn);
            }
            catch (Exception ex)
            {
                log.LogError("ResetHighScore" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
