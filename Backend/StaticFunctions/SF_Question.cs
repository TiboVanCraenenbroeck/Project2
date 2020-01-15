using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class SF_Question
    {
        public static async Task<bool> CheckIfQuestionExistAsync(Guid guidQuestion)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) AS countQuestion FROM TB_Questions WHERE ID=@questionId AND IsDeleted=0";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@questionId", guidQuestion);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            int test = Convert.ToInt32(reader["countQuestion"]);
                            if (Convert.ToInt32(reader["countQuestion"]) == 1)
                            {
                                return true;
                            }
                            else { return false; }
                        }
                        else { return false; }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<bool> ChangeQuestionAsync(Model_Question question)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "UPDATE TB_Questions SET Question=@question, TB_Answers_ID=@answerId, Difficulty=@difficulty WHERE ID=@questionId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@question", question.strQuestion);
                        command.Parameters.AddWithValue("@difficulty", question.intDifficulty);
                        command.Parameters.AddWithValue("@questionId", question.Id);
                        // Check wich answer is correct
                        Guid guidCorrectAnswerId = new Guid();
                        foreach (Model_Answer itemAnswer in question.listAnswer)
                        {
                            if (itemAnswer.blnCorrect == true)
                            {
                                // Search the answerId in the database
                                guidCorrectAnswerId = await SF_SearchAnswer.SearchAnswerIdAsync(itemAnswer.strAnswer);
                                if (guidCorrectAnswerId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                                {
                                    // Put the new answer into the database
                                    guidCorrectAnswerId = await SF_SearchAnswer.AddAnswerAsync(itemAnswer.strAnswer);
                                }
                                break;
                            }
                        }
                        // Check if their is a correct answer
                        if (guidCorrectAnswerId.ToString() != "")
                        {
                            command.Parameters.AddWithValue("@answerId", guidCorrectAnswerId);
                            await command.ExecuteReaderAsync();
                            return true;
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
        public static async Task ChangeAnswersAsync(Model_Question question)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    // Delete all linked answers
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "DELETE FROM TB_Questions_Answers WHERE TB_Questions_ID=@questionId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@questionId", question.Id);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        reader.Close();
                    }
                    foreach (Model_Answer itemAnswer in question.listAnswer)
                    {
                        // Check if the answer is in the database
                        itemAnswer.Id = await SF_SearchAnswer.SearchAnswerIdAsync(itemAnswer.strAnswer);
                        if (itemAnswer.Id == Guid.Parse("00000000-0000-0000-0000-000000000000"))
                        {
                            // Put the new answer into the database
                            itemAnswer.Id = await SF_SearchAnswer.AddAnswerAsync(itemAnswer.strAnswer);
                        }
                        // Put the answerId and the QuestionId in the database
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            string sql = "INSERT INTO TB_Questions_Answers VALUES(@answerID, @questionId)";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@answerId", itemAnswer.Id);
                            command.Parameters.AddWithValue("@questionId", question.Id);
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
        public static async Task<List<Model_Question>> GetQuestionsAsync(Guid guidQuizId)
        {
            try
            {
                List<Model_Question> listQuestions = new List<Model_Question>();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    // Get all the questions from a quiz (subject)
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT ID, TB_Answers_ID, Question, Difficulty FROM TB_Questions WHERE TB_Quizzes_ID=@quizId AND IsDeleted=0";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@quizId", guidQuizId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        int intCountRows = 0;
                        while (reader.Read())
                        {
                            listQuestions.Add(new Model_Question()
                            {
                                Id = Guid.Parse(reader["ID"].ToString()),
                                strQuestion = reader["Question"].ToString(),
                                intDifficulty = Convert.ToInt32(reader["Difficulty"]),
                                listAnswer = new List<Model_Answer>()
                            });
                            listQuestions[intCountRows].listAnswer.Add(new Model_Answer() { Id = Guid.Parse(reader["TB_Answers_ID"].ToString()) });
                            intCountRows++;
                        }
                        reader.Close();
                    }
                    // Get all the answers from the question
                    var rnd = new Random();
                    for (int i = 0; i < listQuestions.Count; i++)
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = connection;
                            string sql = "SELECT TB_Answers.ID AS answerId, TB_Answers.Answer AS answer FROM TB_Questions_Answers INNER JOIN TB_Answers ON TB_Answers.ID = TB_Questions_Answers.TB_Answers_ID WHERE TB_Questions_Answers.TB_Questions_ID=@questionId";
                            command.CommandText = sql;
                            command.Parameters.AddWithValue("@questionId", listQuestions[i].Id);
                            SqlDataReader reader = await command.ExecuteReaderAsync();
                            while (reader.Read())
                            {
                                // Check if the answer is correct
                                if (listQuestions[i].listAnswer[0].Id == Guid.Parse(reader["answerId"].ToString()))
                                {
                                    listQuestions[i].listAnswer[0].strAnswer = reader["answer"].ToString();
                                    listQuestions[i].listAnswer[0].blnCorrect = true;
                                }
                                else
                                {
                                    listQuestions[i].listAnswer.Add(new Model_Answer()
                                    {
                                        Id = Guid.Parse(reader["answerId"].ToString()),
                                        strAnswer = reader["answer"].ToString(),
                                        blnCorrect = false
                                    });
                                }
                            }
                            // Shuffle list of answers
                            listQuestions[i].listAnswer.Shuffle();
                            reader.Close();
                        }
                    }
                }
                return listQuestions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool CheckIfTheirIsACorrectAnswer(Model_Question question)
        {
            bool blCorrectAnswer = false;
            foreach (Model_Answer itemAnswer in question.listAnswer)
            {
                if (itemAnswer.blnCorrect)
                {
                    blCorrectAnswer = true;
                    break;
                }
            }
            return blCorrectAnswer;
        }
        public static async Task DeleteQuesitonAsync(Guid guidQuizId,Guid guidQuestionId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "UPDATE TB_Questions SET IsDeleted=1 WHERE TB_Quizzes_ID=@quizId AND ID=@questionId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@quizId", guidQuizId);
                        command.Parameters.AddWithValue("@questionId", guidQuestionId);
                        await command.ExecuteReaderAsync();
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
