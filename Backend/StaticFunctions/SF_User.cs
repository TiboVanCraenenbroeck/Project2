using Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit;
using SendGrid.Helpers.Mail;

namespace Backend.StaticFunctions
{
    public class SF_User
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
        public static async Task<bool> CheckIfUserExistAsync(Guid guidUserId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) as countUser FROM TB_Users WHERE ID=@userId";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@userId", guidUserId);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            // Check if their is an user in the database
                            if (Convert.ToInt32(reader["countUser"]) == 1)
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
        public static bool CheckFieldsAreFilledIn(Model_User user)
        {
            try
            {
                if (user.strMail.Length > 0 && user.strName.Length > 1 && user.strSurname.Length > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool CheckIfPasswordIsStrongEnough(string strPassword)
        {
            try
            {
                // BRON: https://stackoverflow.com/questions/19605150/regex-for-password-must-contain-at-least-eight-characters-at-least-one-number-a
                Regex rx = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#.?!@$%^&*-]).{8,}$");
                if (rx.IsMatch(strPassword))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task ChangeUserInfoAsync(Model_User user)
        {
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
            {
                // Encrypt everything
                user = Encrypt(user);
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    string sql = "UPDATE TB_Users SET SurName=@surName, LastName=@lastName, Mail=@mail, Password=@password WHERE ID=@userId";
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@surName", user.strSurname);
                    command.Parameters.AddWithValue("@lastName", user.strName);
                    command.Parameters.AddWithValue("@mail", user.strMail);
                    command.Parameters.AddWithValue("@password", user.strPassword);
                    command.Parameters.AddWithValue("@userId", user.Id);
                    await command.ExecuteReaderAsync();
                }
            }
        }
        public static Model_User Encrypt(Model_User user)
        {
            // Encrypt everything
            SF_Aes aes = new SF_Aes();
            user.strSurname = aes.EncryptToBase64String(user.strSurname);
            user.strName = aes.EncryptToBase64String(user.strName);
            user.strMail = aes.EncryptToBase64String(user.strMail);
            user.strPassword = SF_Hash.GenerateSHA512String(user.strPassword);
            return user;
        }
        public static Model_User Decrypt(Model_User user)
        {
            // Decrypt everything
            SF_Aes aes = new SF_Aes();
            user.strSurname = aes.DecryptFromBase64String(user.strSurname);
            user.strName = aes.DecryptFromBase64String(user.strName);
            user.strMail = aes.DecryptFromBase64String(user.strMail);
            return user;
        }
        public static void SendMail(string strMail, string strSubject,string strMessage)
        {
            try
            {
                // https://github.com/jstedfast/MailKit
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Environment.GetEnvironmentVariable("FromMail")));
                message.To.Add(new MailboxAddress(strMail));
                message.Subject = strSubject;

                message.Body = new TextPart("plain")
                {
                    Text = strMessage
                };

                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.sendgrid.net", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(Environment.GetEnvironmentVariable("sendGridAuthentication"), Environment.GetEnvironmentVariable("sendGridPassword"));

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<bool> CheckIfUserExistWithMailAsync(string strMail)
        {
            try
            {
                bool blReturnValue = false;
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT COUNT(ID) AS countUsers FROM TB_Users WHERE Mail=@mail";
                        command.CommandText = sql;
                        command.Parameters.AddWithValue("@mail", strMail);
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            if (Convert.ToInt32(reader["countUsers"]) == 1)
                            {
                                blReturnValue = true;
                            }
                        }
                    }
                }
                return blReturnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetRandomPassword()
        {
            // BRON: https://madskristensen.net/blog/generate-random-password-in-c/
            string strChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            string strPassword = "";
            Random random = new Random();
            for (int i = 0; i < 8; i++)
            {
                strPassword += strChars[random.Next(0, strChars.Length)];
            }
            return strPassword;
        }
        public static async Task ChangePasswordAsync(Model_User user)
        {
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    string sql = "UPDATE TB_Users SET Password=@password WHERE Mail=@mail";
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@password", user.strPassword);
                    command.Parameters.AddWithValue("@mail", user.strMail);
                    await command.ExecuteReaderAsync();
                }
            }
        }
    }
}
