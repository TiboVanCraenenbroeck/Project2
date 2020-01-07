using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.HTTPTriggers
{
    public static class HT_GetSubjects
    {
        [FunctionName("HT_GetSubjects")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/subjects")] HttpRequest req,
            ILogger log)
        {
            try
            {

                List<Model_QuizSubject> listResult = new List<Model_QuizSubject>();

                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT * FROM TB_Quizzes";
                        command.CommandText = sql;
                        //command.Parameters.AddWithValue("@day", day);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            listResult.Add(new Model_QuizSubject()
                            {
                                Id = Guid.Parse(reader["ID"].ToString()),
                                strTitle = reader["title"].ToString(),
                                strDescription = reader["description"].ToString()
                            });

                        }

                        /*while (await reader.ReadAsync())
                        {
                            listResult.Add(new Model_QuizSubject()
                            {
                                Id = Guid.Parse(reader["quiz_id"].ToString()),
                                strTitle = reader["title"].ToString(),
                                strDescription = reader["description"].ToString()
                            });

                        }*/

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
