using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Backend.Models;
using System.Data.SqlClient;

namespace Backend.HTTPTriggers
{
    public static class HT_GetQuestions
    {
        [FunctionName("HT_GetQuestions")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/Questions")] HttpRequest req,
            ILogger log)
        {
            try
            {

                List<Model_AnswerQuestion> listResult = new List<Model_AnswerQuestion>();

                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "select TB_Questions.Question, TB_Answers.Answer from TB_Questions right join TB_Answers on TB_Questions.TB_Answers_ID = TB_Answers.ID;";
                        command.CommandText = sql;
                        //command.Parameters.AddWithValue("@day", day);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            listResult.Add(new Model_AnswerQuestion()
                            {
                                strRightAnswer = reader["Answer"].ToString(),
                                strQuestion = reader["Question"].ToString()
                            });
                        }
                    }
                }
                return new OkObjectResult(listResult);
            }
            catch (Exception ex)
            {
                log.LogError("getsubject " + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
