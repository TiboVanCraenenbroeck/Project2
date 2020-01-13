using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Backend.StaticFunctions
{
    public class SF_TeamFunctions
    {

        public static async Task<Guid> GetTeamIdAsync(Model_Team team)
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
        public static async Task<Guid> TeamExistsAsync(Model_Team team)
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
                        string sql = "SELECT COUNT(ID) as teamCount, ID FROM TB_Teams WHERE TB_Avatars_ID=@avatarId AND Name=@teamName GROUP BY ID";
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

        public static async Task<Guid> AddTeamAsync(Model_Team team)
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

        public static async Task<List<Model_Team>> GetTeamFromGameAsync(Guid guidGameId)
        {
            try
            {
                List<Model_Team> listTeam = new List<Model_Team>();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT TB_Teams.ID AS TeamId, TB_Teams.Name AS TeamName, TB_Avatars.Name AS AvatarName, TB_Avatars.ID AS AvatarId, TB_Avatars.MovingAvatar AS MovingAvatar, TB_Avatars.StaticAvatarSmall AS StaticAvatarSmall, TB_Avatars.StaticAvatarBig AS StaticAvatarBig FROM TB_Games_Teams INNER JOIN TB_Teams ON TB_Teams.ID = TB_Games_Teams.TB_Teams_ID INNER JOIN TB_Avatars ON TB_Avatars.ID = TB_Teams.TB_Avatars_ID WHERE TB_Games_Teams.TB_Games_ID=@gameId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@gameId", guidGameId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            listTeam.Add(new Model_Team()
                            {
                                Id = Guid.Parse(reader["TeamId"].ToString()),
                                strName = reader["TeamName"].ToString(),
                                avatar = new Model_Avatar()
                                {
                                    Id = Guid.Parse(reader["AvatarId"].ToString()),
                                    strName = reader["AvatarName"].ToString(),
                                    strMovingAvatar = reader["MovingAvatar"].ToString(),
                                    strStaticAvatarBig = reader["StaticAvatarBig"].ToString(),
                                    strStaticAvatarSmall = reader["StaticAvatarSmall"].ToString()
                                }
                            });
                        }
                    }
                }
                return listTeam;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
