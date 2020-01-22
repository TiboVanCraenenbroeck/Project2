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
    public static class HT_GetUserInfo
    {
        [FunctionName("HT_GetUserInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/user")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                Model_User userinfo = new Model_User();
                // Check if the user is logged in
                if (await SF_User.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                    {
                        await connection.OpenAsync();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            string sql = "SELECT ID, SurName, LastName, Mail FROM TB_Users WHERE ID=@userId";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@userId", day);
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            if (reader.Read())
                            {
                                userinfo.Id = Guid.Parse(reader["Id"].ToString());
                                userinfo.strSurname = reader["SurName"].ToString();
                                userinfo.strName = reader["LastName"].ToString();
                                userinfo.strMail = reader["Mail"].ToString();
                                // Decrypt the info
                                userinfo = SF_User.Decrypt(userinfo);
                            }
                        }
                    }
                }
                return new OkObjectResult(userinfo);
            }
            catch (Exception ex)
            {
                log.LogError("HT_GetUserInfo" + ex.ToString());
            }
        }
    }
}
