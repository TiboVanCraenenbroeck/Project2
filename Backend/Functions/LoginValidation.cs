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

namespace Backend.Functions
{
    public static class LoginValidation
    {
        [FunctionName("LoginValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/login/{mail}/{password}")] HttpRequest req, string mail, string password,
            ILogger log)
        {
            try
            {
                LoginValidationReturn loginValidationReturn = new LoginValidationReturn();
                // Controleer of er iets ingevuld is
                if (mail.Length > 0 && password.Length > 0)
                {
                    User user = new User();

                    Aes aes = new Aes();
                    user.strMail = aes.EncryptToBase64String(mail);
                    user.strPassword = Hash.GenerateSHA512String(password);

                    // Check if the combination exist in the database

                    // Make a unique cookieId --> UserId + client ip-address
                    var remoteAddress = req.HttpContext.Connection.RemoteIpAddress;
                    string strCookieId = "cb854d19-eed9-4452-b9db-f0eba854454c";
                    user.Id = Guid.Parse(strCookieId);
                    loginValidationReturn.Id = aes.EncryptToBase64String(user.Id.ToString() + remoteAddress.ToString());
                }
                else
                {
                    loginValidationReturn.Id = "ERROR";
                    loginValidationReturn.strErrorMessage = "Gelieve alle velden in te vullen";
                }
                return new OkObjectResult(loginValidationReturn);
            }
            catch (Exception ex)
            {
                log.LogError("Error LoginValidation" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }

    }
}
