using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class SF_GameValidation
    {
        public static async Task<Question> GetRandomQuestionAsync(Guid guidGameId, int intDifficulty)
        {
            try
            {
                Question question = new Question();
                Guid guidCorrectAnswer = new Guid();
                // Get a random question
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT TOP 1 TB_Questions.ID AS questionId, TB_Questions.Question AS question, TB_Questions.TB_Answers_ID AS answerId FROM TB_Games INNER JOIN TB_Questions ON TB_Games.TB_Quizzes_ID = TB_Questions.TB_Quizzes_ID LEFT JOIN TB_Games_Answers ON TB_Games_Answers.TB_Games_ID = TB_Games.ID WHERE TB_Games.ID = @gameId AND TB_Games_Answers.TB_Games_ID IS NULL AND TB_Questions.Difficulty=@difficulty ORDER BY NEWID()";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@gameId", guidGameId);
                        command.Parameters.AddWithValue("@difficulty", intDifficulty);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            question.Id = Guid.Parse(reader["questionId"].ToString());
                            question.strQuestion = reader["question"].ToString();
                            guidCorrectAnswer = Guid.Parse(reader["answerId"].ToString());
                            reader.Close();
                        }
                    }
                    // Get the answers
                    question.listAnswer = new List<Answer>();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT TB_Answers.ID AS answerId, TB_Answers.Answer AS answer FROM TB_Questions_Answers INNER JOIN TB_Answers ON TB_Answers.ID = TB_Questions_Answers.TB_Answers_ID WHERE TB_Questions_Answers.TB_Questions_ID=@questionId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@questionId", question.Id);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            bool blnCorrectAnswer = false;
                            // Check if it is the correcvt answer
                            if (Guid.Parse(reader["answerId"].ToString()) == guidCorrectAnswer)
                            {
                                blnCorrectAnswer = true;
                            }
                            question.listAnswer.Add(new Answer()
                            {
                                Id = Guid.Parse(reader["answerId"].ToString()),
                                strAnswer = reader["answer"].ToString(),
                                blnCorrect = blnCorrectAnswer
                            });
                        }
                        reader.Close();
                    }
                }
                return question;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<bool> GameIdExistsAsync(Guid guidGameId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) as countGames FROM TB_Games WHERE ID=@gameId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@gameId", guidGameId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            if (Convert.ToInt32(reader["countGames"]) == 1)
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
