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

namespace Backend.HTTPTriggers
{
    public static class HT_DeleteUser
    {
        [FunctionName("HT_DeleteUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/user/{userId}")] HttpRequest req, string userId,
            ILogger log)
        {
            try
            {
                // Get the userId
                Guid guidUserId = Guid.Parse(userId);
                string cookies_ID = req.Query["cookie_id"];
                Model_ObjectResultReturn objectResultReturn = new Model_ObjectResultReturn();
                objectResultReturn.Id = "ERROR";
                // Check if the user is logged in
                if (await SF_User.CheckIfUserIsLoggedInAsync(cookies_ID, req.HttpContext.Connection.RemoteIpAddress.ToString()))
                {
                    // Check if the user exists
                    if(await SF_User.CheckIfUserExistAsync(guidUserId))
                    {
                        // Delete the user from the database
                        await SF_User.DeleteUserAsync(guidUserId);
                        objectResultReturn.Id = "true";
                    }
                    else
                    {
                        objectResultReturn.strErrorMessage = "Deze gebruiker bestaat niet";
                    }
                }
                else
                {
                    objectResultReturn.strErrorMessage = "Je bent afgemeld";
                }
                return new OkObjectResult(objectResultReturn);
            }
            catch (Exception ex)
            {
                log.LogError("HT_DeleteUser" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
