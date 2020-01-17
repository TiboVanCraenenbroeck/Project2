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
using System.Data.SqlClient;

namespace Backend.HTTPTriggers
{
    public static class HT_GetTotalScoreFromQuiz
    {
        [FunctionName("HT_GetTotalScoreFromGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/quiz/score/{quizId}")] HttpRequest req, string quizId,
            ILogger log)
        {
            try
            {
                Model_GetScoreFromQuiz getScoreFromQuiz = new Model_GetScoreFromQuiz();
                // Get the total questions from this quiz
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) as countQuestions FROM TB_Questions WHERE TB_Quizzes_ID =@quizId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@quizId", Guid.Parse(quizId));
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            getScoreFromQuiz.intMaxScore = Convert.ToInt32(reader["countQuestions"]);
                        }
                    }
                }
                // Calculate the max total score from this subject
                int intScoreForEachTeam = getScoreFromQuiz.intMaxScore / 2;
                int CountTotalQuestionsForEachQuestion = Convert.ToInt32(intScoreForEachTeam * 0.2);
                int intMaxTotalScoreForEachTeam = Convert.ToInt32((1 * 50 * CountTotalQuestionsForEachQuestion) + (2 * 50 * CountTotalQuestionsForEachQuestion) + (2 * 50 * CountTotalQuestionsForEachQuestion));
                getScoreFromQuiz.intMaxScore = intMaxTotalScoreForEachTeam;
                return new OkObjectResult(getScoreFromQuiz);
            }
            catch (Exception ex)
            {
                log.LogError("HT_GetTotalScoreFromGame" + ex.ToString());
                return new StatusCodeResult(500);
            }
        }
    }
}
