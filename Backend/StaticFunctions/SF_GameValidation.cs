using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class SF_GameValidation
    {
        public static async Task PutScoresIntoDatabaseAsync(Guid guidGameId)
        {
            try
            {
                List<Model_Team> listTeams = await SF_TeamFunctions.GetTeamFromGameAsync(guidGameId);
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    foreach (Model_Team itemTeam in listTeams)
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            string sql = "UPDATE TB_Games_Teams SET Score=@score WHERE TB_Games_ID=@gameId AND TB_Teams_ID=@teamId";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@score", itemTeam.intScore);
                            command.Parameters.AddWithValue("@gameId", guidGameId);
                            command.Parameters.AddWithValue("@teamId", itemTeam.Id);
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<List<Model_Team>> GetScoreFromTeamsAsync(Guid guidGameId, List<Model_Team> listTeams)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    for (int i = 0; i < listTeams.Count; i++)
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            string sql = "SELECT SUM(score) as countScore FROM TB_Games_Answers WHERE TB_Games_ID=@gameId AND TB_Teams_ID=@teamId";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@gameId", guidGameId);
                            command.Parameters.AddWithValue("@teamId", listTeams[i].Id);
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            if (reader.Read())
                            {
                                // Check if the team has a score
                                if (reader["countScore"].ToString() != "")
                                {
                                    listTeams[i].intScore = Convert.ToInt32(reader["countScore"]);
                                }
                                else
                                {
                                    listTeams[i].intScore = 0;

                                }
                            }
                            reader.Close();
                        }
                    }
                }
                return listTeams;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<Model_GameValidation> CheckAnswerAsync(Model_GameValidation modelGameValidation, Guid guidGameId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    // Get score 
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) AS countPoints FROM TB_Questions where TB_Answers_ID=@answerId AND ID=@questionId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@answerId", modelGameValidation.question.listAnswer[0].Id);
                        command.Parameters.AddWithValue("@questionId", modelGameValidation.question.Id);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            modelGameValidation.intNumberOfCorrectAttempts++;
                            //countPoints = If the answer is correct --> return=1 else return=0
                            modelGameValidation.team.intScore = Convert.ToInt32(reader["countPoints"]) * modelGameValidation.intNumberOfCorrectAttempts * 1111;
                        }
                        reader.Close();
                    }
                    // Put the answer into the database
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = " INSERT INTO TB_Games_Answers VALUES(@gameId, @questionId, @answerId, @teamId, @score, @timeNeeded, null)";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@gameId", guidGameId);
                        command.Parameters.AddWithValue("@questionId", modelGameValidation.question.Id);
                        command.Parameters.AddWithValue("@answerId", modelGameValidation.question.listAnswer[0].Id);
                        command.Parameters.AddWithValue("@teamId", modelGameValidation.team.Id);
                        command.Parameters.AddWithValue("@score", modelGameValidation.team.intScore);
                        command.Parameters.AddWithValue("@timeNeeded", modelGameValidation.intTime);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        reader.Close();
                    }
                }
                // Difficulty ++ by 3 correct answers
                if (modelGameValidation.question.intDifficulty < 3)
                {
                    modelGameValidation.question.intDifficulty = modelGameValidation.intNumberOfCorrectAttempts / 3;

                }
                return modelGameValidation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<Model_Question> GetRandomQuestionAsync(Guid guidGameId, int intDifficulty)
        {
            try
            {
                Model_Question question = new Model_Question();
                Guid guidCorrectAnswer = new Guid();
                // Get a random question
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT TOP 1 TB_Questions.ID AS questionId, TB_Questions.Question AS question, TB_Questions.TB_Answers_ID AS answerId FROM TB_Games INNER JOIN TB_Questions ON TB_Games.TB_Quizzes_ID = TB_Questions.TB_Quizzes_ID LEFT JOIN TB_Games_Answers ON TB_Games_Answers.TB_Questions_ID = TB_Games.TB_Quizzes_ID WHERE TB_Games.ID = @gameId AND TB_Games_Answers.TB_Games_ID IS NULL AND TB_Questions.Difficulty=@difficulty AND (SELECT COUNT(*) FROM TB_Games_Answers WHERE TB_Questions_ID=TB_Questions.ID AND TB_Games_ID=TB_Games.ID)=0 AND TB_Questions.IsDeleted=0 ORDER BY NEWID()";
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
                    if (question.Id.ToString() != "00000000-0000-0000-0000-000000000000")
                    {
                        // Get the answers
                        question.listAnswer = new List<Model_Answer>();
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
                                question.listAnswer.Add(new Model_Answer()
                                {
                                    Id = Guid.Parse(reader["answerId"].ToString()),
                                    strAnswer = reader["answer"].ToString(),
                                    blnCorrect = blnCorrectAnswer
                                });
                            }
                            reader.Close();
                        }
                    }
                }
                // Shuffle list of answers
                question.listAnswer.Shuffle();
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
        public static void DeleteHighScores(Guid guidQuizId)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
