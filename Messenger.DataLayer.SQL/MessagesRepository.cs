using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.DataLayer;
using Messenger.Model;
using System.Data.SqlClient;

namespace Messenger.DataLayer.SQL
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly string connectionString;

        public MessagesRepository(string _connectionString)
        {
            connectionString = _connectionString;
        }

        public void DeleteMessage(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Messages WHERE MessageId = @MessageId";
                    command.Parameters.AddWithValue("@MessageId", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Message GetMessage(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
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
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Messages (MessageId, ProfileId, ChatId, MessageText, SendDate, LifeTime, AttachId) VALUES (@MessageId, @ProfileId, @ChatId, @MessageText, @SendDate, @LifeTime, @AttachId)";
                    command.Parameters.AddWithValue("@MessageId", message.MessageId);
                    command.Parameters.AddWithValue("@ProfileId", message.ProfileId);
                    command.Parameters.AddWithValue("@ChatId", message.ChatId);
                    command.Parameters.AddWithValue("@MessageText", message.MessageText);
                    command.Parameters.AddWithValue("@SendDate", message.Date);
                    command.Parameters.AddWithValue("@LifeTime", message.TimeToDestroy);
                    command.Parameters.AddWithValue("@AttachId", message.Attachment);
                    command.ExecuteNonQuery();
                    return message;
                }
            }
        }
    }
}
