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
    public static class HT_CreateNewUserAccount
    {
        [FunctionName("CreateNewUserAccount")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/user")] HttpRequest req,
            ILogger log)
        {
            try
            {
                Model_CreateNewUserAccountReturn createNewUserAccountReturn = new Model_CreateNewUserAccountReturn();
                string cookies_ID = req.Query["cookie_id"];
                //Get data from body
                string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                Model_User newUser = JsonConvert.DeserializeObject<Model_User>(strJson);
                newUser.Id = Guid.NewGuid();
                string test = newUser.Id.ToString();
                // Check if the user is logged in
                if (await SF_User.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Check if all fields are filled in
                    if (newUser.strMail.Length > 0 && newUser.strName.Length > 0 && newUser.strSurname.Length > 0)
                    {
                        // Check if the password is strong
                        if (SF_User.CheckIfPasswordIsStrongEnough(newUser.strPassword))
                        {
                            // Encrypt everything
                            newUser = SF_User.Encrypt(newUser);
                            // Put the new user into the database
                            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                            {
                                await connection.OpenAsync();
                                // Check if the user already exist
                                using (SqlCommand command = new SqlCommand())
                                {
                                    command.Connection = connection;
                                    string sql = "SELECT count(ID) as userCount FROM TB_Users WHERE Mail=@mail AND IsDeleted=0";
                                    command.CommandText = sql;
                                    command.Parameters.AddWithValue("@mail", newUser.strMail);
                                    SqlDataReader reader = await command.ExecuteReaderAsync();
                                    if (reader.Read())
                                    {
                                        if (Convert.ToInt32(reader["userCount"]) == 0)
                                        {
                                            reader.Close();
                                            // Insert the user into the database
                                            using (SqlCommand commandA = new SqlCommand())
                                            {
                                                commandA.Connection = connection;
                                                string sqlA = "INSERT INTO TB_Users(ID, SurName, LastName, Mail, Password) VALUES(@id,@surname,@lastname,@mail,@password)";
                                                commandA.CommandText = sqlA;
                                                commandA.Parameters.AddWithValue("@id", newUser.Id);
                                                commandA.Parameters.AddWithValue("@surname", newUser.strSurname);
                                                commandA.Parameters.AddWithValue("@lastname", newUser.strName);
                                                commandA.Parameters.AddWithValue("@mail", newUser.strMail);
                                                commandA.Parameters.AddWithValue("@password", newUser.strPassword);
                                                await commandA.ExecuteReaderAsync();
                                            }
                                            createNewUserAccountReturn.blSucceeded = true;
                                        }
                                        else
                                        {
                                            createNewUserAccountReturn.blSucceeded = false;
                                            createNewUserAccountReturn.strMessage = "Er bestaat al een gebruiker met dit mailadres";
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            createNewUserAccountReturn.blSucceeded = false;
                            createNewUserAccountReturn.strMessage = "Je wachtwoord moet minstens 8 karakters, 1 nummer, 1 hoofdletter, 1 gewone letter en een speciaal teken (.?) bevatten";
                        }
                    }
                    else
                    {
                        createNewUserAccountReturn.blSucceeded = false;
                        createNewUserAccountReturn.strMessage = "Gelieve alle velden in te vullen";
                    }
                }
                else
                {
                    createNewUserAccountReturn.blSucceeded = false;
                    createNewUserAccountReturn.strMessage = "Je bent afgemeld";
                }
                return new OkObjectResult(createNewUserAccountReturn);
            }
            catch (Exception ex)
            {
                log.LogError("CreateNewUserAccount: " + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
