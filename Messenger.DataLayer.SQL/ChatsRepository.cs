using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Messenger.Model;

namespace Messenger.DataLayer.SQL 
{
    public class ChatsRepository : IChatsRepository
    {
        private readonly string connectionString;
        private readonly IProfilesRepository profilesRepository;

        public ChatsRepository(string _connectionString, IProfilesRepository _profilesRepository)
        {
            connectionString = _connectionString;
            profilesRepository = _profilesRepository;
        }

        public Chat CreateChat(Chat chat)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "INSERT INTO Chats (ChatId, ChatName) VALUES (@ChatId, @ChatName)";
                        command.Parameters.AddWithValue("@ChatId", chat.ChatId);
                        command.Parameters.AddWithValue("@ChatName", chat.ChatName);

                        command.ExecuteNonQuery();
                    }

                    foreach (var profileId in chat.ChatMembers)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "INSERT INTO ChatMembers (ChatId, ProfileId) VALUES (@ChatId, @Id)";
                            command.Parameters.AddWithValue("@ChatId", chat.ChatId);
                            command.Parameters.AddWithValue("@Id", profileId);
                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    chat.ChatMembers = chat.ChatMembers.Select(x => profilesRepository.GetProfile(x.Id));
                    return chat;
                }
            }
        }

        public Chat GetChat(Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ChatId, ChatName FROM Chats WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Чат с chatId {chatId} не найден");
                        return new Chat
                        {
                            ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                            ChatName = reader.GetString(reader.GetOrdinal("ChatName")),
                            ChatMembers = GetChatMembers(reader.GetGuid(reader.GetOrdinal("ChatId")))
                        };
                    }
                }
            }
        }

        public void DeleteChat(Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM ChatMembers WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.ExecuteNonQuery();
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Chats WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.ExecuteNonQuery();
                }
            }
        }


        // Additional methods.
        public IEnumerable<Chat> GetChat(string chatName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ChatId, ChatName FROM Chats WHERE ChatName = @ChatName";
                    command.Parameters.AddWithValue("@ChatName", chatName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Чат с chatName {chatName} не найден");
                        while (reader.Read())
                        {
                            yield return new Chat
                            {
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                ChatName = reader.GetString(reader.GetOrdinal("ChatName")),
                                ChatMembers = GetChatMembers(reader.GetGuid(reader.GetOrdinal("ChatId")))
                            };
                        }
                    }
                }
            }
        }

        public void AddChatMember(Guid userId, Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO ChatMembers (ChatId, ProfileId) VALUES (@ChatId, @ProfileId)";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@ProfileId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteChatMember(Guid userId, Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM ChatMembers WHERE ChatId = @ChatId AND ProfileId = @ProfileId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@ProfileId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Profile> GetChatMembers(Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ProfileId FROM ChatMembers WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            yield return profilesRepository.GetProfile(reader.GetGuid(reader.GetOrdinal("ProfileId")));
                    }
                }
            }
        }

    }
}
