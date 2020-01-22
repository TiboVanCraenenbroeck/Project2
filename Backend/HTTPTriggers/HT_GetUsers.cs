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
using System.Collections.Generic;
using Backend.StaticFunctions;
using System.Data.SqlClient;

namespace Backend.HTTPTriggers
{
    public static class HT_GetUsers
    {
        [FunctionName("HT_GetUsers")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                List<Model_User> listUsers = new List<Model_User>();
                // Check if the user is logged in
                if (await SF_User.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Open encryption
                    SF_Aes aes = new SF_Aes();
                    // Get all the users from the database
                    using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                    {
                        await connection.OpenAsync();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            string sql = "SELECT ID, SurName, LastName, Mail FROM TB_Users";
                            command.CommandText = sql;
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            while (reader.Read())
                            {
                                // Create an object
                                Model_User user = new Model_User()
                                {
                                    Id = Guid.Parse(reader["SurName"].ToString()),
                                    strSurname = reader["SurName"].ToString(),
                                    strName = reader["LastName"].ToString(),
                                    strMail = reader["Mail"].ToString()

                                };
                                // Decrypt the object
                                user = SF_User.Decrypt(user);
                                // Put the encrypted user into the list
                                listUsers.Add(user);
                                /*listUsers.Add(new Model_User()
                                {
                                    Id = Guid.Parse(reader["ID"].ToString()),
                                    strSurname = aes.DecryptFromBase64String(reader["SurName"].ToString()),
                                    strName = aes.DecryptFromBase64String(reader["LastName"].ToString()),
                                    strMail = aes.DecryptFromBase64String(reader["Mail"].ToString())
                                });*/
                            }
                        }
                    }
                }
                return new OkObjectResult(listUsers);
            }
            catch (Exception ex)
            {
                log.LogError("HT_GetUsers" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
