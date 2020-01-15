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
    public static class HT_DeleteQuestion
    {
        [FunctionName("HT_DeleteQuestion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/question/{QuizId}/{QuestionId}")] HttpRequest req, string QuizId, string QuestionId,
            ILogger log)
        {
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                Guid guidQuizId = Guid.Parse(QuizId);
                Guid guidQuestionId = Guid.Parse(QuestionId);
                Model_ObjectResultReturn ObjectResultReturn = new Model_ObjectResultReturn();
                // Check if the user is logged in
                if (await SF_IsUserLoggedIn.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Check if the Quiz exists
                    if (await SF_QuizExists.CheckIfQuizExistsAsync(guidQuizId))
                    {
                        // Check if the Question exists
                        if (await SF_Question.CheckIfQuestionExistAsync(guidQuestionId))
                        {
                            // Delete the question (set column IsDelete TRUE)
                            await SF_Question.DeleteQuesitonAsync(guidQuizId, guidQuestionId);
                            ObjectResultReturn.Id = "true";
                        }
                        else
                        {
                            ObjectResultReturn.Id = "ERROR";
                            ObjectResultReturn.strErrorMessage = "Deze vraag bestaat niet";
                        }
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
                log.LogError("HT_DeleteQuestion" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
