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
using System.Data.SqlClient;

namespace Backend.HTTPTriggers
{
    public static class AddQuestion
    {
        [FunctionName("AddQuestion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/question/{QuizId}")] HttpRequest req, string QuizId,
            ILogger log)
        {
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                ObjectResultReturn objectResultReturn = new ObjectResultReturn();
                Guid guidQuizId = Guid.Parse(QuizId);
                // Check if the user is logged in
                if (await IsUserLoggedIn.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    //Ophalen van de data
                    string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                    Question newQuestion = JsonConvert.DeserializeObject<Question>(strJson);
                    newQuestion.Id = Guid.NewGuid();

                    // Check if the subject exists in the database
                    if (await QuizExists.CheckIfQuizExistsAsync(guidQuizId))
                    {
                        // Make the answer
                        Guid guidCorrectAnswer = new Guid();
                        Guid guidCheck = Guid.Parse("00000000-0000-0000-0000-000000000000");
                        for (int i = 0; i < newQuestion.listAnswer.Count; i++)
                        {
                            newQuestion.listAnswer[i].Id = await SearchAnswer.SearchAnswerIdAsync(newQuestion.listAnswer[i].strAnswer);
                            // Check if the answer already exists
                            if (newQuestion.listAnswer[i].Id == guidCheck)
                            {
                                newQuestion.listAnswer[i].Id = await SearchAnswer.AddAnswerAsync(newQuestion.listAnswer[i].strAnswer);
                            }
                            // Check if this answer is the correct anwser
                            if (newQuestion.listAnswer[i].blnCorrect == true)
                            {
                                guidCorrectAnswer = newQuestion.listAnswer[i].Id;
                            }
                        }
                        // Check if their is a coorect answer
                        if (guidCorrectAnswer != guidCheck)
                        {
                            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                            {
                                await connection.OpenAsync();
                                //Put the question into the database
                                using (SqlCommand commandA = new SqlCommand())
                                {
                                    commandA.Connection = connection;
                                    string sqlA = "INSERT INTO TB_Questions VALUES(@questionId, @quizId, @answerId, @question, @difficulty)";
                                    commandA.CommandText = sqlA;
                                    commandA.Parameters.AddWithValue("@questionId", newQuestion.Id);
                                    commandA.Parameters.AddWithValue("@quizId", guidQuizId);
                                    commandA.Parameters.AddWithValue("@answerId", guidCorrectAnswer);
                                    commandA.Parameters.AddWithValue("@question", newQuestion.strQuestion);
                                    commandA.Parameters.AddWithValue("@difficulty", newQuestion.intDifficulty);
                                    SqlDataReader readerA = await commandA.ExecuteReaderAsync();
                                    readerA.Close();
                                }
                                //Put all the answers with the question in the database
                                foreach (Answer itemAnswer in newQuestion.listAnswer)
                                {
                                    using (SqlCommand commandB = new SqlCommand())
                                    {
                                        commandB.Connection = connection;
                                        string sqlA = "INSERT INTO TB_Questions_Answers VALUES(@answerID, @quizId)";
                                        commandB.CommandText = sqlA;
                                        commandB.Parameters.AddWithValue("@answerID", itemAnswer.Id);
                                        commandB.Parameters.AddWithValue("@quizId", newQuestion.Id);
                                        SqlDataReader readerB = await commandB.ExecuteReaderAsync();
                                        readerB.Close();
                                    }
                                }
                                objectResultReturn.Id = newQuestion.Id.ToString();
                            }
                        }
                        else
                        {
                            objectResultReturn.Id = "ERROR";
                            objectResultReturn.strErrorMessage = "Er is geen juist antwoord aangeduid";
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
                log.LogError("AddQuestion" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
