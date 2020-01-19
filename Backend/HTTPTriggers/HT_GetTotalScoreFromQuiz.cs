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
using System.Collections.Generic;

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
                List<int> listScores = new List<int>();
                // Get the total questions from this quiz
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT (SELECT COUNT(ID) FROM TB_Questions WHERE Difficulty=0 AND IsDeleted=0 AND TB_Quizzes_ID=@quizId) as countQuestionsEasy, (SELECT COUNT(ID) FROM TB_Questions WHERE Difficulty=1 AND IsDeleted=0 AND TB_Quizzes_ID=@quizId) as countQuestionsMedium, (SELECT COUNT(ID) FROM TB_Questions WHERE Difficulty=2 AND IsDeleted=0 AND TB_Quizzes_ID=@quizId) as countQuestionsHard";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@quizId", Guid.Parse(quizId));
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            listScores.Add(Convert.ToInt32(reader["countQuestionsEasy"]));
                            listScores.Add(Convert.ToInt32(reader["countQuestionsMedium"]));
                            listScores.Add(Convert.ToInt32(reader["countQuestionsHard"]));
                        }
                    }
                }
                // Calculate the max total score from this subject
                int intMaxTotalScoreForEachTeam = Convert.ToInt32((1 * 50 * listScores[0]) + (2 * 50 * listScores[1]) + (2 * 50 * listScores[2]));
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
