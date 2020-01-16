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
    public static class HT_GetAvatar
    {
        [FunctionName("HT_GetAvatar")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/avatars")] HttpRequest req,
            ILogger log)
        {
            try
            {

                List<Model_Avatar> listResult = new List<Model_Avatar>();

                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT * FROM TB_Avatars ORDER BY Name";
                        command.CommandText = sql;
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            listResult.Add(new Model_Avatar()
                            {
                                Id = Guid.Parse(reader["ID"].ToString()),
                                strName = reader["Name"].ToString()
                            });

                        }
                    }
                }
                return new OkObjectResult(listResult);
            }
            catch (Exception ex)
            {
                log.LogError("GetAvatar " + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}

