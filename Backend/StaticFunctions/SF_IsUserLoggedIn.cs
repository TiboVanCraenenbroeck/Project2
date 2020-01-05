using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class SF_IsUserLoggedIn
    {
        public static async Task<bool> CheckIfUserIsLoggedInAsync(string strCookie_ID, string strIpAddress)
        {
            try
            {
                SF_Aes aesCookies = new SF_Aes(1);
                string strDecryptedCookie = aesCookies.DecryptFromBase64String(strCookie_ID);
                string[] strCookieSplit = strDecryptedCookie.Split("!!!");
                Guid guidUserId = Guid.Parse(strCookieSplit[0]);
                // Check if the ip-address are the same
                if (strIpAddress == strCookieSplit[1])
                {
                    Model_User Result = new Model_User();
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
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
