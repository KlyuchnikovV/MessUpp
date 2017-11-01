using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.DataLayer;
using Messenger.Model;
using System.Data.SqlClient;
using NLog;

namespace Messenger.DataLayer.SQL
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly string connectionString;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MessagesRepository(string _connectionString)
        {
            connectionString = _connectionString;
        }

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
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    return;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Удаление сообщения {0}", id);
                    command.CommandText = "DELETE FROM Messages WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exc)
                    {
                        logger.Error(exc.Message);
                        return;
                    }
                }
            }
        }

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
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    return null;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Получение сообщения {0}", id);
                    command.CommandText = "SELECT TOP(1) * FROM Messages WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Сообщение с id {id} не найден");

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
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    return null;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Отправка сообщения с параметрами: ИД сообщения = {0}, ИД профиля = {1}, ИД чата = {2}, Текст = {3}, Дата отправки = {4}, Время жизни = {5}, ИД приложения = {6}",
                        message.MessageId, message.ProfileId), message.ChatId, message.MessageText, message.Date, message.TimeToDestroy, message.Attachment);
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
                    catch (SqlException exc)
                    {
                        logger.Error(exc.Message);
                        return null;
                    }
                    return message;
                }
            }
        }
    }
}
