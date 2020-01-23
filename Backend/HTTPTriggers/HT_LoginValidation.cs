using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Backend.StaticFunctions;
using Backend.Models;
using System.Data.SqlClient;
using System.Web;

namespace Backend.Functions
{
    public static class HT_LoginValidation
    {
        [FunctionName("LoginValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/login/{mail}/{password}")] HttpRequest req, string mail, string password,
            ILogger log)
        {
            try
            {
                //LoginValidationReturn loginValidationReturn = new LoginValidationReturn();
                Model_ObjectResultReturn objectResultReturn = new Model_ObjectResultReturn();
                // Controleer alle velden ingevuld zijn
                if (mail.Length > 0 && password.Length > 0)
                {
                    Model_User user = new Model_User();

                    SF_Aes aes = new SF_Aes();
                    user.strMail = aes.EncryptToBase64String(mail);
                    user.strPassword = SF_Hash.GenerateSHA512String(password);

                    // Check if the combination exist in the database
                    using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                    {
                        await connection.OpenAsync();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            string sql = "SELECT ID, COUNT(*) as usersCount FROM TB_Users WHERE Mail=@mail AND Password=@password AND IsDeleted=0 GROUP BY ID";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@mail", user.strMail);
                            command.Parameters.AddWithValue("@password", user.strPassword);
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            if (reader.Read())
                            {
                                if (Convert.ToInt32(reader["usersCount"]) == 1)
                                {
                                    user.Id = Guid.Parse(reader["ID"].ToString());
                                    // Make a unique cookieId --> UserId + client ip-address
                                    var remoteAddress = req.HttpContext.Connection.RemoteIpAddress;
                                    SF_Aes aesCookies = new SF_Aes(1);
                                    objectResultReturn.Id = aesCookies.EncryptToBase64String(user.Id.ToString()+ "!!!" + remoteAddress.ToString());
                                }
                                else
                                {
                                    objectResultReturn.Id = "ERROR";
                                    objectResultReturn.strErrorMessage = "Deze combinatie (mail en wachtwoord) vinden we niet terug in onze database";
                                }
                            }
                            else
                            {
                                objectResultReturn.Id = "ERROR";
                                objectResultReturn.strErrorMessage = "Uw wachtwoord en of mailadres is fout";
                            }
                        }
                    }
                }
                else
                {
                    objectResultReturn.Id = "ERROR";
                    objectResultReturn.strErrorMessage = "Gelieve alle velden in te vullen";
                }
                return new OkObjectResult(objectResultReturn);
            }
            catch (Exception ex)
            {
                log.LogError("Error LoginValidation" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
