using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Backend.Models;
using Backend.StaticFunctions;

namespace Backend.HTTPTriggers
{
    public static class HT_GetTeamsFromGame
    {
        [FunctionName("HT_GetTeamsFromGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/game/teams/{gameId}")] HttpRequest req, string gameId,
            ILogger log)
        {
            try
            {
                List<Model_Team> listTeam = await SF_TeamFunctions.GetTeamFromGameAsync(Guid.Parse(gameId));
                return new OkObjectResult(listTeam);

            }
            catch (Exception ex)
            {
                log.LogError("HT_GetTeamsFromGame" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
