using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class TeamFunctions
    {

        public static async Task<Guid> GetTeamIdAsync(Team team)
        {
            try
            {
                Guid guidTeamId = new Guid();
                guidTeamId = await TeamExistsAsync(team);
                if (guidTeamId.ToString() == "00000000-0000-0000-0000-000000000000")
                {
                    guidTeamId = await AddTeamAsync(team);
                }
                return guidTeamId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<Guid> TeamExistsAsync(Team team)
        {
            try
            {
                Guid guidTeamId = new Guid();
                // Check if the team already exists
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) as teamCount, ID FROM TB_Teams WHERE TB_Avatars_ID=@avatarId AND Name=@teamName";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@avatarId", team.avatar.Id);
                        command.Parameters.AddWithValue("@teamName", team.strName);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            if (Convert.ToInt32(reader["teamCount"]) == 1)
                            {
                                guidTeamId = Guid.Parse(reader["ID"].ToString());
                            }
                        }
                    }
                }
                return guidTeamId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Guid> AddTeamAsync(Team team)
        {
            try
            {
                Guid guidTeamId = Guid.NewGuid();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "INSERT INTO TB_Teams VALUES(@id, @avatarId, @name)";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@id", guidTeamId);
                        command.Parameters.AddWithValue("@avatarId", team.avatar.Id);
                        command.Parameters.AddWithValue("@name", team.strName);
                        await command.ExecuteReaderAsync();
                    }
                }
                return guidTeamId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
