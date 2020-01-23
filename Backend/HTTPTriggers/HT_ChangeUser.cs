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

namespace Backend.HTTPTriggers
{
    public static class HT_ChangeUser
    {
        [FunctionName("HT_ChangeUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/user")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string cookies_ID = req.Query["cookie_id"];
                Model_ObjectResultReturn objectResultReturn = new Model_ObjectResultReturn();
                //Get the data from the body
                string strJson = await new StreamReader(req.Body).ReadToEndAsync();
                Model_User newModel_User = JsonConvert.DeserializeObject<Model_User>(strJson);
                // Check if the user is logged in
                if (await SF_User.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Check if the user exists
                    if (await SF_User.CheckIfUserExistAsync(newModel_User.Id))
                    {
                        // Check if all the input-fields are filled in
                        if (SF_User.CheckFieldsAreFilledIn(newModel_User))
                        {
                            // Check if the current user of this account is
                            SF_Aes aesCookies = new SF_Aes(1);
                            string strDecryptedCookie = aesCookies.DecryptFromBase64String(cookies_ID);
                            string[] strCookieSplit = strDecryptedCookie.Split("!!!");
                            Guid guidUserId = Guid.Parse(strCookieSplit[0]);
                            if (guidUserId == newModel_User.Id)
                            {
                                // Encrypt the data
                                Model_User encryptedUser = SF_User.Encrypt(newModel_User);
                                // Check if the user has filled in a password
                                if (newModel_User.strPassword != null && newModel_User.strPassword != "")
                                {
                                    // Check if the password is strong enenough
                                    if (SF_User.CheckIfPasswordIsStrongEnough(newModel_User.strPassword))
                                    {
                                        // Change the data into the database + encrypt the data
                                        await SF_User.ChangeUserInfoAsync(newModel_User);
                                        // Change the password
                                        await SF_User.ChangePasswordAsync(encryptedUser);
                                        objectResultReturn.Id = "true";
                                    }
                                    else
                                    {
                                        objectResultReturn.Id = "ERROR";
                                        objectResultReturn.strErrorMessage = "Je wachtwoord moet minstens 8 karakters, 1 nummer, 1 hoofdletter, 1 gewone letter en een speciaal teken (.?) bevatten";
                                    }
                                }
                                else
                                {
                                    // Change the data into the database + encrypt the data
                                    await SF_User.ChangeUserInfoAsync(newModel_User);
                                    objectResultReturn.Id = "true";
                                }
                            }
                            else
                            {
                                objectResultReturn.Id = "ERROR";
                                objectResultReturn.strErrorMessage = "Je kan enkel gegevens van je eigen account wijzigen";
                            }
                        }
                        else
                        {
                            objectResultReturn.Id = "ERROR";
                            objectResultReturn.strErrorMessage = "Gelieve alle velden in te vullen";
                        }
                    }
                    else
                    {
                        objectResultReturn.Id = "ERROR";
                        objectResultReturn.strErrorMessage = "Deze gebruiker bestaat niet";
                    }
                }
                else
                {
                    objectResultReturn.Id = "ERROR";
                    objectResultReturn.strErrorMessage = "Je bent afgemeld";
                }
                return new OkObjectResult(objectResultReturn);
            }
            catch (Exception ex)
            {
                log.LogError("HT_ChangeUser" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
