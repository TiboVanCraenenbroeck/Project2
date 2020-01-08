using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) AS countQuestion FROM TB_Questions WHERE ID=@questionId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@questionId", guidQuestion);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
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
        public static async Task ChangeQuestionAsync(Model_Question question)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("ConnectionString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "UPDATE TB_Questions SET Question=@question, TB_Answers_ID=@answerId, Difficulty=@difficulty WHERE ID=@questionId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@question", question.strQuestion);
                        command.Parameters.AddWithValue("@difficulty", question.intDifficulty);
                        command.Parameters.AddWithValue("@questionId", question.intDifficulty);

                        //command.Parameters.AddWithValue("@answerId", NOG BEKIJKEN);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            /*listResult.Add(new ClassName()
                            {
                                AantalBezoekers = int.Parse(reader["AantalBezoekers"].ToString()),
                            });*/
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void GetQuestions(Guid guidQuizId)
        {

        }
    }
}
