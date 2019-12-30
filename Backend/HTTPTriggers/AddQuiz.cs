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
    public static class AddQuiz
    {
        [FunctionName("AddQuiz")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"post", Route = "v1/subject")] HttpRequest req, 
            ILogger log)
        {
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                ObjectResultReturn objectResultReturn = new ObjectResultReturn();
                //Ophalen van de data
                string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                QuizSubject newQuizSubject = JsonConvert.DeserializeObject<QuizSubject>(strJson);
                newQuizSubject.Id = Guid.NewGuid();
                newQuizSubject.dtDateTime = DateTime.Now;

                // Check if the user is logged in
                if (await IsUserLoggedIn.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Insert the subject onto the database
                    using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                    {
                        await connection.OpenAsync();
                        using (SqlCommand command = new SqlCommand())
                        {
                            // Check if the subject exists
                            command.Connection = connection;
                            string sql = "SELECT COUNT(ID) as quizCount FROM TB_Quizzes WHERE Title=@subject";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@subject", newQuizSubject.strTitle);
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            if (reader.Read())
                            {
                                if (Convert.ToInt32(reader["quizCount"]) == 0)
                                {
                                    reader.Close();
                                    // Insert the new subject into the database
                                    using (SqlCommand commandA = new SqlCommand())
                                    {
                                        commandA.Connection = connection;
                                        string sqlA = "INSERT INTO TB_Quizzes VALUES(@id,@subject,@description,@dateTime)";
                                        commandA.CommandText = sqlA;
                                        commandA.Parameters.AddWithValue("@id", newQuizSubject.Id);
                                        commandA.Parameters.AddWithValue("@subject", newQuizSubject.strTitle);
                                        commandA.Parameters.AddWithValue("@description", newQuizSubject.strDescription);
                                        commandA.Parameters.AddWithValue("@dateTime", newQuizSubject.dtDateTime);
                                        await commandA.ExecuteReaderAsync();
                                        objectResultReturn.Id = newQuizSubject.Id.ToString();
                                    }
                                }
                                else
                                {
                                    objectResultReturn.Id = "ERROR";
                                    objectResultReturn.strErrorMessage = "Dit onderwerp bestaat al";
                                }
                            }
                            else
                            {
                                reader.Close();
                                // Insert the new subject into the database
                                using (SqlCommand commandA = new SqlCommand())
                                {
                                    commandA.Connection = connection;
                                    string sqlA = "INSERT INTO TB_Quizzes VALUES(@id,@subject,@description,@dateTime)";
                                    commandA.CommandText = sqlA;
                                    commandA.Parameters.AddWithValue("@id", newQuizSubject.Id);
                                    commandA.Parameters.AddWithValue("@subject", newQuizSubject.strTitle);
                                    commandA.Parameters.AddWithValue("@description", newQuizSubject.strDescription);
                                    commandA.Parameters.AddWithValue("@dateTime", newQuizSubject.dtDateTime);
                                    await commandA.ExecuteReaderAsync();
                                    objectResultReturn.Id = newQuizSubject.Id.ToString();
                                }
                            }
                        }
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
                log.LogError("AddQuiz" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
