using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Messenger.Model;
using NLog;

namespace Messenger.DataLayer.SQL
{
    /// <summary>
    ///     Реализаия интерфейса для методов работы с таблицей "Сообщения".
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class MessagesRepository : IMessagesRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;
        private Guid _idOfDeleting;

        /// <summary>
        ///     Инициализация строки подключения для работы с таблицей "Сообщения".
        /// </summary>
        /// <param name="connectionString">Строка подключения.</param>
        public MessagesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc />
        public Message CreateMessage(Message message)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Создание сообщения...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    message.MessageId = Guid.NewGuid();
                    message.Date = DateTime.Now;
                    command.CommandText =
                        "INSERT INTO Messages (MessageId, ProfileId, ChatId, MessageText, SendDate, LifeTime, AttachId, IsRead) " +
                        "VALUES (@MessageId, @ProfileId, @ChatId, @MessageText, @SendDate, @LifeTime, @AttachId, @IsRead)";
                    command.Parameters.AddWithValue("@MessageId", message.MessageId);
                    command.Parameters.AddWithValue("@ProfileId", message.ProfileId);
                    command.Parameters.AddWithValue("@ChatId", message.ChatId);
                    command.Parameters.AddWithValue("@MessageText", message.MessageText);
                    command.Parameters.AddWithValue("@SendDate", message.Date);
                    command.Parameters.AddWithValue("@LifeTime", message.TimeToDestroy);
                    command.Parameters.AddWithValue("@AttachId", message.Attachment);
                    command.Parameters.AddWithValue("@IsRead", "0");
                    Logger.Info("Создание сообщения...");
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                    return message;
                }
            }
        }

        /// <inheritdoc />
        public Message GetMessage(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение сообщения...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Messages WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);
                    Logger.Info($"Получение сообщения {id}");
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new Exception($"Сообщение с ИД {id} не найден");
                        return new Message
                        {
                            MessageId = reader.GetGuid(reader.GetOrdinal("MessageId")),
                            ProfileId = reader.GetGuid(reader.GetOrdinal("ProfileId")),
                            ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                            MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                            Date = reader.GetDateTime(reader.GetOrdinal("SendDate")),
                            TimeToDestroy = reader.GetInt32(reader.GetOrdinal("LifeTime")),
                            Attachment = reader.GetGuid(reader.GetOrdinal("AttachId")),
                            IsRead = reader.GetBoolean(reader.GetOrdinal("IsRead"))
                        };
                    }
                }
            }
        }

        /// <inheritdoc />
        public void DeleteMessage(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Удаление сообщения...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Messages WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);
                    Logger.Info($"Удаление сообщения {id}");
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<Message> GetMessages(Guid chatId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение сообщений чата...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Messages WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    Logger.Info($"Получение сообщений чата {chatId}");
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
                                IsRead = reader.GetBoolean(reader.GetOrdinal("IsRead"))
                            };
                    }
                }
            }
        }

        /// <inheritdoc />
        public int CountMessages(Guid chatId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Подсчет сообщений чата...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Count(*) as count FROM Messages WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    Logger.Info($"Получение сообщений чата {chatId}");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            return reader.GetInt32(reader.GetOrdinal("count"));
                    }
                }
            }
            return 0;
        }

        /// <inheritdoc />
        public int CountReadMessages(Guid chatId, Guid personId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Подсчет прочитанных сообщений...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT COUNT(*) AS count FROM Messages WHERE ChatId = @ChatId and ProfileId = @ProfileId AND IsRead = '1'";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@ProfileId", personId);
                    Logger.Info($"Подсчет прочитанных сообщений {chatId}");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            return reader.GetInt32(reader.GetOrdinal("count"));
                    }
                }
            }
            return 0;
        }

        /// <inheritdoc />
        public IEnumerable<Message> FindMessages(IEnumerable<string> names, Guid profileId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Поиск по сообщениям.");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                foreach (var name in names)
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * FROM Messages WHERE MessageText LIKE @Name " +
                                              "AND ChatId IN (SELECT ChatId FROM ChatMembers WHERE ProfileId = @ProfileId) " +
                                              "AND ProfileId IN (SELECT ProfileId FROM ChatMembers WHERE ChatId IN (" +
                                              "SELECT ChatId FROM ChatMembers WHERE ProfileId = @ProfileId));";
                        command.Parameters.AddWithValue("@Name", "%" + name + "%");
                        command.Parameters.AddWithValue("@ProfileId", profileId);
                        Logger.Info($"Поиск сообщений по строке {name}...");
                        SqlDataReader reader;
                        try
                        {
                            reader = command.ExecuteReader();
                        }
                        catch (SqlException exception)
                        {
                            Logger.Error(exception.Message);
                            throw;
                        }
                        while (reader.Read())
                            yield return new Message
                            {
                                MessageId = reader.GetGuid(reader.GetOrdinal("MessageId")),
                                ProfileId = reader.GetGuid(reader.GetOrdinal("ProfileId")),
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                                Date = reader.GetDateTime(reader.GetOrdinal("SendDate")),
                                TimeToDestroy = reader.GetInt32(reader.GetOrdinal("LifeTime")),
                                Attachment = reader.GetGuid(reader.GetOrdinal("AttachId"))
                            };
                        reader.Close();
                    }
            }
        }

        /// <inheritdoc />
        public void CheckUndestroyedMessages(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Проверка сообщений не удаленных автоматически...");
                var list = new List<Message>();
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT * FROM Messages WHERE LifeTime <> 0 AND ChatId = @Id AND IsRead = '1'";
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(new Message
                            {
                                MessageId = reader.GetGuid(reader.GetOrdinal("MessageId")),
                                ProfileId = reader.GetGuid(reader.GetOrdinal("ProfileId")),
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                MessageText = reader.GetString(reader.GetOrdinal("MessageText")),
                                Date = reader.GetDateTime(reader.GetOrdinal("SendDate")),
                                TimeToDestroy = reader.GetInt32(reader.GetOrdinal("LifeTime")),
                                Attachment = reader.GetGuid(reader.GetOrdinal("AttachId"))
                            });
                    }
                }
                foreach (var message in list)
                {
                    if (!(message.Date.TimeOfDay.TotalSeconds + message.TimeToDestroy <
                          DateTime.Now.TimeOfDay.TotalSeconds)) continue;
                    message.MessageText = "\\Сообщение удалено.\\";
                    message.Attachment = Guid.Parse("00000000-0000-0000-0000-000000000001");
                    message.TimeToDestroy = 0;
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE Messages SET MessageText = @MessageText, LifeTime = @LifeTime, " +
                                              "AttachId = @AttachId WHERE MessageId = @MessageId";
                        command.Parameters.AddWithValue("@MessageId", message.MessageId);
                        command.Parameters.AddWithValue("@MessageText", message.MessageText);
                        command.Parameters.AddWithValue("@LifeTime", message.TimeToDestroy);
                        command.Parameters.AddWithValue("@AttachId", message.Attachment);
                        Logger.Info("Удаление сообщения...");
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException exception)
                        {
                            Logger.Error(exception.Message);
                            throw;
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public void UpdateMessageRead(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Установка флага IsRead...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Messages SET IsRead = 'true' WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);
                    Logger.Info($"Получение сообщения {id}");
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Destroy(Message message)
        {
            if (message.TimeToDestroy <= 0)
                return;
            var thread = new Thread(SelfDestroy);
            _idOfDeleting = message.MessageId;
            thread.Start();
        }

        /// <summary>
        ///     Обновляет сообщение с текстом об удалении сообщения и вложения.
        /// </summary>
        private void SelfDestroy()
        {
            var message = GetMessage(_idOfDeleting);
            Logger.Debug("Активация отсчета до самоудаления сообщения...");
            Thread.Sleep(message.TimeToDestroy * 1000);

            message.MessageText = "Сообщение удалено.";
            message.Attachment = Guid.Parse("00000000-0000-0000-0000-000000000001");
            message.TimeToDestroy = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Messages SET MessageText = @MessageText, LifeTime = @LifeTime, " +
                                          "AttachId = @AttachId WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", message.MessageId);
                    command.Parameters.AddWithValue("@MessageText", message.MessageText);
                    command.Parameters.AddWithValue("@LifeTime", message.TimeToDestroy);
                    command.Parameters.AddWithValue("@AttachId", message.Attachment);
                    Logger.Info("Удаление сообщения...");
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                }
            }
        }
    }
}