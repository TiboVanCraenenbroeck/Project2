using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class IsUserLoggedIn
    {
        public async Task<bool> CheckIfUserIsLoggedInAsync(Guid guidUserId)
        {
            try
            {

                User Result = new User();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) as userCount FROM TB_Users WHERE ID=@UserId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@UserId", guidUserId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            if (Convert.ToInt32(reader["userCount"]) == 1)
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
    }
}
