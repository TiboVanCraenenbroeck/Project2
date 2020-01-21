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
    public static class HT_PasswordForgotten
    {
        [FunctionName("HT_PasswordForgotten")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/user/password/{mail}")] HttpRequest req, string mail,
            ILogger log)
        {
            try
            {
                Model_ObjectResultReturn objectResultReturn = new Model_ObjectResultReturn();
                objectResultReturn.Id = "ERROR";
                Model_User user = new Model_User();
                user.strMail = mail;
                user.strName = "";
                user.strSurname = "";
                user.strPassword = "";
                user = SF_User.Encrypt(user);
                // Check if the user exists

                if (await SF_User.CheckIfUserExistWithMailAsync(user.strMail))
                {
                    // Create a random password
                    string strPassword = SF_User.GetRandomPassword();
                    // Hash the password
                    user.strPassword = SF_Hash.GenerateSHA512String(strPassword);
                    // Change the password in the database
                    await SF_User.ChangePasswordAsync(user);
                    // Send a mail to the user with a new password
                    string strMailBody = $"Beste\n\n Je nieuwe wachtwoord is: {strPassword}\nGelieve dit wachtwoord zo snel mogelijk te wijzigen.\n\nSportieve groeten\nMove For Fortune Groep 3";
                    SF_User.SendMail(mail, "Wachtwoord vergeten - Move For Fortune Groep 3", strMailBody);
                    objectResultReturn.Id = "true";
                }
                else
                {
                    objectResultReturn.strErrorMessage = "Je mailadres is fout";
                }
                return new OkObjectResult(objectResultReturn);
            }
            catch (Exception ex)
            {
                log.LogError("HT_PasswordForgotten" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
