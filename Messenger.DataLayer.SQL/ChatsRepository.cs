using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Messenger.Model;
using NLog;

namespace Messenger.DataLayer.SQL 
{
    public class ChatsRepository : IChatsRepository
    {
        private readonly string connectionString;
        private readonly IProfilesRepository profilesRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ChatsRepository(string _connectionString, IProfilesRepository _profilesRepository)
        {
            connectionString = _connectionString;
            profilesRepository = _profilesRepository;
        }

        public Chat CreateChat(Chat chat)
        {
            logger.Debug("Создание чата...");
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    return null;
                }
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        chat.ChatId = Guid.NewGuid();
                        logger.Info("Создание чата: ИД = {0}, Имя = {1}", chat.ChatId, chat.ChatName);
                        command.Transaction = transaction;
                        command.CommandText = "INSERT INTO Chats (ChatId, ChatName) VALUES (@ChatId, @ChatName)";
                        command.Parameters.AddWithValue("@ChatId", chat.ChatId);
                        command.Parameters.AddWithValue("@ChatName", chat.ChatName);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException exc)
                        {
                            logger.Error(exc.Message);
                            return null;
                        }
                    }

                    foreach (var profile in chat.ChatMembers)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            logger.Info("Добавление членов чата в таблицу...");
                            command.Transaction = transaction;
                            command.CommandText = "INSERT INTO ChatMembers (ProfileId, ChatId) VALUES (@Id, @ChatId)";
                            command.Parameters.AddWithValue("@Id", profile.Id);
                            command.Parameters.AddWithValue("@ChatId", chat.ChatId);
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlException exc)
                            {
                                logger.Error(exc.Message);
                                return null;
                            }
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
                logger.Debug("Получение чата...");
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
                    logger.Info("Получение чата с id {0}", chatId);
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
                logger.Debug("Удаление чата...");
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
                    logger.Info("Удаение всех пользователей из чата {0}", chatId);
                    command.CommandText = "DELETE FROM ChatMembers WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
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
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Удаление чата {0}", chatId);
                    command.CommandText = "DELETE FROM Chats WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
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


        // Additional methods.
        public IEnumerable<Chat> GetChat(string chatName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Получение чата по имени...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    yield break;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Получение чата {0}", chatName);
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
                logger.Debug("Добавление порльзователя к чату...");
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
                    logger.Info("Добавление порльзователя к чату: ИД пользователя = {0}, ИД чата = {1}", userId, chatId);
                    command.CommandText = "INSERT INTO ChatMembers (ChatId, ProfileId) VALUES (@ChatId, @ProfileId)";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@ProfileId", userId);
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

        public void DeleteChatMember(Guid userId, Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Удаление пользователя из чата...");
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
                    logger.Info("Удаление пользователя {0} из чата {1}", userId, chatId);
                    command.CommandText = "DELETE FROM ChatMembers WHERE ChatId = @ChatId AND ProfileId = @ProfileId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@ProfileId", userId);
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

        public IEnumerable<Profile> GetChatMembers(Guid chatId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Получение пользователей чата...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    yield break;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Получение пользователей чата {0}", chatId);
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
