using System;
using System.Collections.Generic;
using System.Linq;
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

        // Создание чата. //
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
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
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
                        catch (SqlException exception)
                        {
                            logger.Error(exception.Message);
                            throw exception;
                        }
                    }
                    logger.Info($"Чат {chat.ChatName} успешно создан!");

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
                            catch (SqlException exception)
                            {
                                logger.Error(exception.Message);
                                throw exception;
                            }
                        }
                        logger.Info($"Пользователь с {profile.Login} добавлен в чат {chat.ChatName}!");
                    }

                    transaction.Commit();
                    chat.ChatMembers = chat.ChatMembers.Select(x => profilesRepository.GetProfile(x.Id));
                    logger.Info($"Создание чата завершено.");
                    return chat;
                }
            }
        }

        // Получение чата. //
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
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Получение чата с id {0}", chatId);
                    command.CommandText = "SELECT ChatId, ChatName FROM Chats WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new Exception($"Чат с chatId {chatId} не найден");
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

        // Удаление чата. //
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
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
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
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }
                logger.Info($"Удаление пользователей из чата {chatId} завершено.");
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Удаление чата {0}", chatId);
                    command.CommandText = "DELETE FROM Chats WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
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
                logger.Info($"Удаление чата {chatId} завершено.");
            }
        }

        // Добавление пользователя в чат. // TODO: добавить создание запроса на добавление в чат???
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
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
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
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }
                logger.Info($"Пользователь {userId} добавлен в чат {chatId}");
            }
        }

        // Удаление пользователя из чата. //
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
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
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
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }
                logger.Info($"Пользователь {userId} удален из чата {chatId}");
            }
        }

        // Получение пользователей чата. //
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
                    logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw exception;
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
                logger.Info($"Получение пользователей чата {chatId} завершено.");
            }
        }

        // Поиск чатов по набору строк. // 
        public IEnumerable<Chat> FindChats(String[] tokens, Guid profileId)
        {
            logger.Debug("Поиск чатов.");
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
                foreach (var token in tokens)
                {
                    using (var command = connection.CreateCommand())
                    {

                        logger.Info($"Поиск чатов по строке '{token}'");
                        command.CommandText = "SELECT Chats.ChatId, Chats.ChatName FROM Chats Join ChatMembers on Chats.ChatId = ChatMembers.ChatId WHERE Chats.ChatName Like @Name And ProfileId = @ProfileId";
                        command.Parameters.AddWithValue("@Name", "%" + token + "%");
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
                            yield return new Chat
                            {
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                ChatName = reader.GetString(reader.GetOrdinal("ChatName")),
                                ChatMembers = GetChatMembers(reader.GetGuid(reader.GetOrdinal("ChatId")))
                            };
                        }
                        reader.Close();
                    }
                    logger.Info($"Получены чаты по строке {token}.");
                }
                logger.Info($"Завершение поиска.");
            }
        }
    }
}