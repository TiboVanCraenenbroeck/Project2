using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class QuizExists
    {
        public static async Task<bool> CheckIfQuizExistsAsync(Guid guidQuizId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) as quizCount FROM TB_Quizzes WHERE ID=@quizId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@quizId", guidQuizId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            if (Convert.ToInt32(reader["quizCount"]) == 1)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
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
