using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class SF_SearchAnswer
    {
        public static async Task<Guid> SearchAnswerIdAsync(string strAnswer)
        {
            Guid guidAnswerId = new Guid();
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    string sql = "SELECT ID FROM TB_Answers WHERE Answer = @answer";
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@answer", strAnswer);
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        guidAnswerId = Guid.Parse(reader["ID"].ToString());
                    }
                }
            }
            return guidAnswerId;
        }

        public static async Task<Guid> AddAnswerAsync(string strAnswer)
        {
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    Guid guidAnswer = Guid.NewGuid();
                    command.Connection = connection;
                    string sql = "INSERT INTO TB_Answers(ID, Answer) VALUES(@id, @answer)";
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@id", guidAnswer);
                    command.Parameters.AddWithValue("@answer", strAnswer);
                    await command.ExecuteReaderAsync();
                    return guidAnswer;
                }
            }
        }
    }
}
