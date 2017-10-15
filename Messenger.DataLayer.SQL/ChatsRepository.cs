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

        public Chat CreateChat(IEnumerable<Guid> chatMembers, string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var chat = new Chat
                    {
                        ChatId = Guid.NewGuid(),
                        ChatName = name
                    };

                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "INSERT INTO Chats (ChatId, ChatName) VALUES (@ChatId, @ChatName)";
                        command.Parameters.AddWithValue("@ChatId", chat.ChatId);
                        command.Parameters.AddWithValue("@ChatName", chat.ChatName);

                        command.ExecuteNonQuery();
                    }

                    foreach (var profileId in chatMembers)
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
                    chat.ChatMembers = chatMembers.Select(x => profilesRepository.GetProfileById(x));
                    return chat;
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

        /*public Chat GetChat(string chatName)
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
                        return new Chat
                        {
                            ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                            ChatName = reader.GetString(reader.GetOrdinal("ChatName")),
                            ChatMembers = GetChatMembers(reader.GetGuid(reader.GetOrdinal("ChatId")))
                        };
                    }
                }
            }
        }*/

        public IEnumerable<Chat> GetProfileChats(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT ChatMembers.ChatId, ChatName FROM ChatMembers INNER JOIN Chats ON ChatMembers.ChatId = Chats.ChatId WHERE ChatMembers.ProfileId = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
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


        private IEnumerable<Profile> GetChatMembers(Guid chatId)
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
                            yield return profilesRepository.GetProfileById(reader.GetGuid(reader.GetOrdinal("ProfileId")));
                    }
                }
            }
        }

    }
}
