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
    public static class HT_ChangeQuestion
    {
        [FunctionName("HT_ChangeQuestion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/question/{QuizId}")] HttpRequest req, string QuizId,
            ILogger log)
        {
            try
            {
                Guid guidQuizId = Guid.Parse(QuizId);
                string cookies_ID = req.Query["cookie_id"];
                Model_ObjectResultReturn objectResultReturn = new Model_ObjectResultReturn();
                //Ophalen van de data
                string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                Model_Question updateQuestion = JsonConvert.DeserializeObject<Model_Question>(strJson);


                // Check if the user is logged in
                if (await SF_IsUserLoggedIn.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    //Check if the quizz exist
                    if (await SF_QuizExists.CheckIfQuizExistsAsync(guidQuizId))
                    {
                        //Check if the question exists
                        if (await SF_Question.CheckIfQuestionExistAsync(updateQuestion.Id))
                        {
                            //Change the question-title + correct answer in the database
                            // Check if their is a correct answer
                            if (SF_Question.CheckIfTheirIsACorrectAnswer(updateQuestion))
                            {
                                if (await SF_Question.ChangeQuestionAsync(updateQuestion))
                                {
                                    // Change the answers
                                    await SF_Question.ChangeAnswersAsync(updateQuestion);
                                    objectResultReturn.Id = updateQuestion.Id.ToString();
                                }
                                else
                                {
                                    objectResultReturn.Id = "ERROR";
                                    objectResultReturn.strErrorMessage = "Je moet een correct antwoord aanduiden";
                                }
                            }
                            else
                            {
                                objectResultReturn.Id = "ERROR";
                                objectResultReturn.strErrorMessage = "Je moet 1 juist antwoord aanduiden";
                            }
                        }
                        else
                        {
                            objectResultReturn.Id = "ERROR";
                            objectResultReturn.strErrorMessage = "Deze vraag bestaat niet";
                        }
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
                log.LogError("HT_ChangeQuestion" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
