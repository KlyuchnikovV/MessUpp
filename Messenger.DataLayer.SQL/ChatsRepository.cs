using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Messenger.Model;
using NLog;

namespace Messenger.DataLayer.SQL
{
    /// <summary>
    ///     Реализация интерфейса для методов работы с таблицей "Чаты".
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class ChatsRepository : IChatsRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly string _connectionString;
        private readonly IProfilesRepository _profilesRepository;

        /// <summary>
        ///     Инициализация строки подключения для работы с таблицей "Чаты".
        /// </summary>
        /// <param name="connectionString">Строкаподключения.</param>
        /// <param name="profilesRepository">Методы работы с таблицей "Профили".</param>
        public ChatsRepository(string connectionString, IProfilesRepository profilesRepository)
        {
            _connectionString = connectionString;
            _profilesRepository = profilesRepository;
        }

        /// <inheritdoc />
        public Chat CreateChat(Chat chat)
        {
            Logger.Debug("Создание чата...");
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
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        chat.ChatId = Guid.NewGuid();
                        Logger.Info("Создание чата...");
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
                            Logger.Error(exception.Message);
                            throw;
                        }
                    }
                    Logger.Info($"Чат {chat.ChatName} успешно создан!");
                    foreach (var profile in chat.ChatMembers)
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "INSERT INTO ChatMembers (ProfileId, ChatId) VALUES (@Id, @ChatId)";
                            command.Parameters.AddWithValue("@Id", profile);
                            command.Parameters.AddWithValue("@ChatId", chat.ChatId);
                            Logger.Info("Добавление членов чата в таблицу...");
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
                        Logger.Info($"Пользователь {profile} добавлен в чат {chat.ChatName}!");
                    }
                    transaction.Commit();
                    chat.ChatMembers = chat.ChatMembers.Select(x => _profilesRepository.GetProfile(x).Id);
                    Logger.Info("Создание чата завершено.");
                    return chat;
                }
            }
        }

        /// <inheritdoc />
        public Chat GetChat(Guid chatId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение чата...");
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
                    command.CommandText = "SELECT ChatId, ChatName FROM Chats WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    Logger.Info($"Получение чата с id {chatId}");
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

        /// <inheritdoc />
        public void DeleteChat(Guid chatId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Удаление чата...");
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
                    command.CommandText = "DELETE FROM ChatMembers WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    Logger.Info($"Удаление всех пользователей из чата {chatId}");
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
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Messages WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    Logger.Info($"Удаление всех сообщений из чата {chatId}");
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
                Logger.Info($"Удаление пользователей из чата {chatId} завершено.");
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Chats WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    Logger.Info($"Удаление чата {chatId}");
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
                Logger.Info($"Удаление чата {chatId} завершено.");
            }
        }

        /// <inheritdoc />
        public void AddChatMember(Guid profileId, Guid chatId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Добавление порльзователя к чату...");
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
                    command.CommandText = "INSERT INTO ChatMembers (ChatId, ProfileId) VALUES (@ChatId, @ProfileId)";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@ProfileId", profileId);
                    Logger.Info("Добавление порльзователя к чату...");
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
                Logger.Info($"Пользователь {profileId} добавлен в чат {chatId}");
            }
        }

        /// <inheritdoc />
        public void DeleteChatMember(Guid profileId, Guid chatId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Удаление пользователя из чата...");
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
                    Logger.Info($"Удаление пользователя {profileId} из чата {chatId}");
                    command.CommandText = "DELETE FROM ChatMembers WHERE ChatId = @ChatId AND ProfileId = @ProfileId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@ProfileId", profileId);
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
                Logger.Info($"Пользователь {profileId} удален из чата {chatId}");
                using (var command = connection.CreateCommand())
                {
                    var count = 0;
                    Logger.Info($"Проверка чата {chatId} на наличие пользоателей.");
                    command.CommandText = "SELECT COUNT(*) AS count FROM ChatMembers WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            count = reader.GetInt32(reader.GetOrdinal("count"));
                    }
                    if (count > 0)
                        return;
                    Logger.Info($"Удаление чата {chatId} - нет пользователей.");
                    DeleteChat(chatId);
                }
            }
        }

        /// <inheritdoc />
        public IEnumerable<Guid> GetChatMembers(Guid chatId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение пользователей чата...");
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
                    command.CommandText = "SELECT ProfileId FROM ChatMembers WHERE ChatId = @ChatId";
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    Logger.Info($"Получение пользователей чата {chatId}");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            yield return reader.GetGuid(reader.GetOrdinal("ProfileId"));
                    }
                }
                Logger.Info($"Получение пользователей чата {chatId} завершено.");
            }
        }

        /// <inheritdoc />
        public IEnumerable<Chat> FindChats(IEnumerable<string> tokens, Guid profileId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Поиск чатов.");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                foreach (var token in tokens)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT Chats.ChatId, Chats.ChatName FROM Chats Join ChatMembers ON" +
                                              " Chats.ChatId = ChatMembers.ChatId WHERE Chats.ChatName Like @Name AND" +
                                              " ProfileId = @ProfileId";
                        command.Parameters.AddWithValue("@Name", "%" + token + "%");
                        command.Parameters.AddWithValue("@ProfileId", profileId);
                        Logger.Info($"Поиск чатов по строке '{token}'");
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
                            yield return new Chat
                            {
                                ChatId = reader.GetGuid(reader.GetOrdinal("ChatId")),
                                ChatName = reader.GetString(reader.GetOrdinal("ChatName")),
                                ChatMembers = GetChatMembers(reader.GetGuid(reader.GetOrdinal("ChatId")))
                            };
                        reader.Close();
                    }
                    Logger.Info($"Получены чаты по строке {token}.");
                }
                Logger.Info("Завершение поиска.");
            }
        }
    }
}