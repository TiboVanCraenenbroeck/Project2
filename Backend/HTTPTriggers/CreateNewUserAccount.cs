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
    public static class CreateNewUserAccount
    {
        [FunctionName("CreateNewUserAccount")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/user")] HttpRequest req,
            ILogger log)
        {
            try
            {
                CreateNewUserAccountReturn createNewUserAccountReturn = new CreateNewUserAccountReturn();

                //Get data from body
                string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                User newUser = JsonConvert.DeserializeObject<User>(strJson);
                newUser.Id = Guid.NewGuid();
                string test = newUser.Id.ToString();

                // Check if all fields are filled in
                if (newUser.strMail.Length > 3 && newUser.strName.Length > 3 && newUser.strSurname.Length > 3)
                {
                    // Check if the password is strong
                    if (newUser.strPassword.Length >= 9)
                    {
                        // Encrypt everything
                        Aes aes = new Aes();
                        newUser.strSurname = aes.EncryptToBase64String(newUser.strSurname);
                        newUser.strName = aes.EncryptToBase64String(newUser.strName);
                        newUser.strMail = aes.EncryptToBase64String(newUser.strMail);
                        newUser.strPassword = Hash.GenerateSHA512String(newUser.strPassword);
                        // Put the new user into the database
                        using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                        {
                            await connection.OpenAsync();
                            // Check if the user already exist
                            using (SqlCommand command = new SqlCommand())
                            {
                                command.Connection = connection;
                                string sql = "SELECT count(ID) as userCount FROM TB_Users WHERE Mail=@mail";
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
                                            string sqlA = "INSERT INTO TB_Users VALUES(@id,@surname,@lastname,@mail,@password)";
                                            commandA.CommandText = sqlA;
                                            commandA.Parameters.AddWithValue("@id", newUser.Id);
                                            commandA.Parameters.AddWithValue("@surname", newUser.strName);
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
                        createNewUserAccountReturn.strMessage = "Het wachtwoord moet minstens 9 karakters lang zijn";
                    }
                }
                else
                {
                    createNewUserAccountReturn.blSucceeded = false;
                    createNewUserAccountReturn.strMessage = "Gelieve alle velden in te vullen";
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
