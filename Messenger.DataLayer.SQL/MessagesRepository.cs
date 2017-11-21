﻿using System;
using System.Collections.Generic;
using Messenger.Model;
using System.Data.SqlClient;
using NLog;
using System.Threading;

namespace Messenger.DataLayer.SQL
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly string connectionString;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        Thread thread;
        Guid idOfDeleting;

        public MessagesRepository(string _connectionString)
        {
            connectionString = _connectionString;
        }

        // Создает новое сообщение в чате. //
        public Message SendMessage(Message message)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Создание сообщения...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    message.MessageId = Guid.NewGuid();
                    message.Date = System.DateTime.Now;
                    logger.Info("Отправка сообщения с параметрами: ИД сообщения = {0}, ИД профиля = {1}, ИД чата = {2}, Текст = {3}, Дата отправки = {4}, Время жизни = {5}, ИД приложения = {6}",
                        message.MessageId, message.ProfileId, message.ChatId, message.MessageText, message.Date, message.TimeToDestroy, message.Attachment);
                    command.CommandText = "INSERT INTO Messages (MessageId, ProfileId, ChatId, MessageText, SendDate, LifeTime, AttachId) VALUES (@MessageId, @ProfileId, @ChatId, @MessageText, @SendDate, @LifeTime, @AttachId)";
                    command.Parameters.AddWithValue("@MessageId", message.MessageId);
                    command.Parameters.AddWithValue("@ProfileId", message.ProfileId);
                    command.Parameters.AddWithValue("@ChatId", message.ChatId);
                    command.Parameters.AddWithValue("@MessageText", message.MessageText);
                    command.Parameters.AddWithValue("@SendDate", message.Date);
                    command.Parameters.AddWithValue("@LifeTime", message.TimeToDestroy);
                    command.Parameters.AddWithValue("@AttachId", message.Attachment);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }

                    if(message.TimeToDestroy > 0)
                    {
                        thread = new Thread(SelfDestroy);
                        idOfDeleting = message.MessageId;
                        thread.Start();
                    }

                    return message;
                }
            }
        }

        // Получает сообщение по ИД. //
        public Message GetMessage(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Получение сообщения...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info($"Получение сообщения {id}");
                    command.CommandText = "SELECT TOP(1) * FROM Messages WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new Exception($"Сообщение с id {id} не найден");

                        return new Message
                        {
                            MessageId = reader.GetGuid(reader.GetOrdinal("MessageId")),
                            ProfileId = reader.GetGuid(reader.GetOrdinal("ProfileId")),
                            ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                            MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                            Date = reader.GetDateTime(reader.GetOrdinal("SendDate")),
                            TimeToDestroy = reader.GetInt32(reader.GetOrdinal("LifeTime")),
                            Attachment = reader.GetGuid(reader.GetOrdinal("AttachId")),
                        };
                    }
                }
            }
        }

        // Удаляет сообщение по ИД. //
        public void DeleteMessage(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Удаление сообщения...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info($"Удаление сообщения {id}");
                    command.CommandText = "DELETE FROM Messages WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }
            }
        }

        // Получает все сообщения чата. //
        public IEnumerable<Message> GetMessages(Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Получение сообщений чата...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Получение сообщений чата {0}", chatId);
                    command.CommandText = "SELECT * FROM Messages WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            yield return new Message
                            {
                                MessageId = reader.GetGuid(reader.GetOrdinal("MessageId")),
                                ProfileId = reader.GetGuid(reader.GetOrdinal("ProfileId")),
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                                Date = reader.GetDateTime(reader.GetOrdinal("SendDate")),
                                TimeToDestroy = reader.GetInt32(reader.GetOrdinal("LifeTime")),
                                Attachment = reader.GetGuid(reader.GetOrdinal("AttachId")),
                            };
                    }
                }
            }
        }

        // Подсчитывает все сообщения чата(например для мониторинга новых сообщений). //
        public int CountMessages(Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Подсчет сообщений чата...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info($"Получение сообщений чата {chatId}");
                    command.CommandText = "SELECT Count(*) as count FROM Messages WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            return (reader.GetInt32(reader.GetOrdinal("count")));
                    }
                }
            }
            return 0;
        }

        // Ищет все сообщения . //
        public IEnumerable<Message> FindMessages(String[] names, Guid profileId)
        {
            logger.Debug("Поиск по сообщениям.");
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                foreach (var name in names)
                {
                    using (var command = connection.CreateCommand())
                    {

                        logger.Info($"Поиск сообщений по строке {name}");
                        command.CommandText = "select * from Messages Where MessageText Like @Name and ChatId in (SELECT ChatId from ChatMembers where ProfileId = @ProfileId) and ProfileId IN (SELECT ProfileId from ChatMembers where ChatId in (SELECT ChatId from ChatMembers where ProfileId = @ProfileId));";
                        command.Parameters.AddWithValue("@Name", "%" + name + "%");
                        command.Parameters.AddWithValue("@ProfileId", profileId);
                        SqlDataReader reader;
                        try
                        {
                            reader = command.ExecuteReader();
                        }
                        catch (SqlException exception)
                        {
                            logger.Error(exception.Message);
                            throw exception;
                        }
                        while (reader.Read())
                        {
                            yield return new Message
                            {
                                MessageId = reader.GetGuid(reader.GetOrdinal("MessageId")),
                                ProfileId = reader.GetGuid(reader.GetOrdinal("ProfileId")),
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                                Date = reader.GetDateTime(reader.GetOrdinal("SendDate")),
                                TimeToDestroy = reader.GetInt32(reader.GetOrdinal("LifeTime")),
                                Attachment = reader.GetGuid(reader.GetOrdinal("AttachId")),
                            };
                        }
                        reader.Close();
                    }
                }
            }
        }

        // Обновляет сообщение с текстом об удалении сообщения и вложения.
        public void SelfDestroy()
        {
            Message message = GetMessage(idOfDeleting);
            logger.Debug("Активация отсчета до самоудаления сообщения...");
            Thread.Sleep(message.TimeToDestroy * 1000);

            message.MessageText = "Сообщение удалено.";
            message.Attachment = Guid.Parse("00000000-0000-0000-0000-000000000001");
            message.TimeToDestroy = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Удаление сообщения с параметрами: ИД сообщения = {0}, ИД профиля = {1}, ИД чата = {2}, Текст = {3}, Дата отправки = {4}, Время жизни = {5}, ИД приложения = {6}",
                        message.MessageId, message.ProfileId, message.ChatId, message.MessageText, message.Date, message.TimeToDestroy, message.Attachment);
                    command.CommandText = "UPDATE Messages SET MessageText = @MessageText, LifeTime = @LifeTime, AttachId = @AttachId WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", message.MessageId);
                    command.Parameters.AddWithValue("@MessageText", message.MessageText);
                    command.Parameters.AddWithValue("@LifeTime", message.TimeToDestroy);
                    command.Parameters.AddWithValue("@AttachId", message.Attachment);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }
            }
        }

        public void CheckUndestroyedMessages(Guid id)
        {
            logger.Debug("Проверка сообщений не удаленных автоматически...");
            List<Message> list = new List<Message>();
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Messages WHERE LifeTime <> 0 And ChatId = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Message
                            {
                                MessageId = reader.GetGuid(reader.GetOrdinal("MessageId")),
                                ProfileId = reader.GetGuid(reader.GetOrdinal("ProfileId")),
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                                Date = reader.GetDateTime(reader.GetOrdinal("SendDate")),
                                TimeToDestroy = reader.GetInt32(reader.GetOrdinal("LifeTime")),
                                Attachment = reader.GetGuid(reader.GetOrdinal("AttachId")),
                            });
                        }
                    }
                }
                foreach(var message in list)
                {
                    if(message.Date.TimeOfDay.TotalSeconds + message.TimeToDestroy < DateTime.Now.TimeOfDay.TotalSeconds)
                    {
                        message.MessageText = "\\Сообщение удалено.\\";
                        message.Attachment = Guid.Parse("00000000-0000-0000-0000-000000000001");
                        message.TimeToDestroy = 0;
                        using (var command = connection.CreateCommand())
                        {
                            logger.Info("Удаление сообщения с параметрами: ИД сообщения = {0}, ИД профиля = {1}, ИД чата = {2}, Текст = {3}, Дата отправки = {4}, Время жизни = {5}, ИД приложения = {6}",
                                message.MessageId, message.ProfileId, message.ChatId, message.MessageText, message.Date, message.TimeToDestroy, message.Attachment);
                            command.CommandText = "UPDATE Messages SET MessageText = @MessageText, LifeTime = @LifeTime, AttachId = @AttachId WHERE MessageId = @MessageId";
                            command.Parameters.AddWithValue("@MessageId", message.MessageId);
                            command.Parameters.AddWithValue("@MessageText", message.MessageText);
                            command.Parameters.AddWithValue("@LifeTime", message.TimeToDestroy);
                            command.Parameters.AddWithValue("@AttachId", message.Attachment);
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlException exception)
                            {
                                logger.Error(exception.Message);
                                throw exception;
                            }
                        }
                    }
                }
            }
        }
    }
}
