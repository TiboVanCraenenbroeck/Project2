﻿using Backend.Models;
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
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
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
        public static async Task<List<Model_Question>> GetQuestionsAsync(Guid guidQuizId)
        {
            try
            {
                List<Model_Question> listQuestions = new List<Model_Question>();
                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("SQL_ConnectionsString")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        string sql = "SELECT ID, TB_Answers_ID, Question, Difficulty FROM TB_Questions WHERE TB_Quizzes_ID=@quizId";
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
    }
}
