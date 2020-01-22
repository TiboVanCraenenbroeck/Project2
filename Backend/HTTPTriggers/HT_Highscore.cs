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
    public static class HT_Highscore
    {
        [FunctionName("Highscore")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/highscores")] HttpRequest req,
            ILogger log)
        {
            try
            {

                List<Model_Highscore> listResult = new List<Model_Highscore>();

                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT top(10) TB_Teams.ID AS teamID, TB_Teams.Name as NameTeam, TB_Games_Teams.Score AS score , TB_Avatars.Name FROM TB_Teams INNER JOIN TB_Games_Teams ON TB_Teams.ID = TB_Games_Teams.TB_Teams_ID INNER JOIN TB_Games ON TB_Games.ID = TB_Games_Teams.TB_Games_ID inner join TB_Avatars ON TB_Teams.TB_Avatars_ID = TB_Avatars.ID WHERE TB_Games.IsDeleted = 0 order by score Desc";
                        command.CommandText = sql;
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            listResult.Add(new Model_Highscore()
                            {
                                Id = Guid.Parse(reader["teamID"].ToString()),
                                TeamName = reader["NameTeam"].ToString(),
                                score = int.Parse(reader["score"].ToString()),
                                image = reader["Name"].ToString()
                            });
                        }
                    }
                }
                return new OkObjectResult(listResult);
            }
            catch (Exception ex)
            {
                log.LogError("Highscore " + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
