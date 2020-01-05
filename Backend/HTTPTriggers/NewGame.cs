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
            try
            {
                ObjectResultReturn objectResultReturn = new ObjectResultReturn();
                Guid guidQuizId = Guid.Parse(QuizId);

                // Check if the quiz exist
                if (await QuizExists.CheckIfQuizExistsAsync(guidQuizId))
                {
                    //Ophalen van de data
                    string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                    Game newGame = JsonConvert.DeserializeObject<Game>(strJson);
                    newGame.Id = Guid.NewGuid();
                    // Check if the teams haven't the same name and character
                    if (newGame.teams[0].strName != newGame.teams[1].strName && newGame.teams[0].avatar.Id != newGame.teams[1].avatar.Id)
                    {
                        // Get the team_id's
                        for (int i = 0; i < newGame.teams.Count; i++)
                        {
                            newGame.teams[i].Id = await TeamFunctions.GetTeamIdAsync(newGame.teams[i]);
                        }
                        // Insert the game into the database
                        using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                        {
                            await connection.OpenAsync();
                            using (SqlCommand command = new SqlCommand())
                            {
                                command.Connection = connection;
                                string sql = "INSERT INTO TB_Games VALUES(@id, @QuizId, @dateTime)";
                                command.CommandText = sql;
                                command.Parameters.AddWithValue("@id", newGame.Id);
                                command.Parameters.AddWithValue("@QuizId", guidQuizId);
                                command.Parameters.AddWithValue("@dateTime", DateTime.Now);
                                SqlDataReader reader = await command.ExecuteReaderAsync();
                                reader.Close();
                            }
                            //Link teams to the game
                            foreach (Team itemTeam in newGame.teams)
                            {
                                using (SqlCommand command = new SqlCommand())
                                {
                                    command.Connection = connection;
                                    string sql = "INSERT INTO TB_Games_Teams VALUES(@gameId, @teamId, 0)";
                                    command.CommandText = sql;
                                    command.Parameters.AddWithValue("@gameId", newGame.Id);
                                    command.Parameters.AddWithValue("@teamId", itemTeam.Id);
                                    SqlDataReader reader = await command.ExecuteReaderAsync();
                                    reader.Close();
                                }
                            }
                            objectResultReturn.Id = newGame.Id.ToString();
                        }
                    }
                    else
                    {
                        objectResultReturn.Id = "ERROR";
                        objectResultReturn.strErrorMessage = "Beide teams moeten een verschillende naam en avatar hebben";
                    }
                }
                else
                {
                    objectResultReturn.Id = "ERROR";
                    objectResultReturn.strErrorMessage = "Dit onderwerp bestaat niet";
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
