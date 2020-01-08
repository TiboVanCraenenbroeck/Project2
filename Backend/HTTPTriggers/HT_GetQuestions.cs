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
using System.Data.SqlClient;
using Backend.StaticFunctions;

namespace Backend.HTTPTriggers
{
    public static class HT_GetQuestions
    {
        [FunctionName("HT_GetQuestions")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/questions/{quizId}")] HttpRequest req, string quizId,
            ILogger log)
        {
            try
            {
                List<Model_Question> listResult = await SF_Question.GetQuestionsAsync(Guid.Parse(quizId));
                return new OkObjectResult(listResult);
            }
            catch (Exception ex)
            {
                log.LogError("getsubject " + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
