using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Messenger.Model;
using NLog;

namespace Messenger.DataLayer.SQL
{
    /// <summary>
    ///     Реализация интерфейса для методов работы с таблицей "Профили".
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class ProfilesRepository : IProfilesRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IChatsRepository _chatsRepository;
        private readonly string _connectionString;

        /// <summary>
        ///     Инициализация строки подключения для работы с таблицей "Профили".
        /// </summary>
        /// <param name="connectionString">Строка подключения.</param>
        public ProfilesRepository(string connectionString)
        {
            _connectionString = connectionString;
            _chatsRepository = new ChatsRepository(connectionString, this);
        }

        /// <inheritdoc />
        public Profile CreateProfile(Profile newProfile)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Создание пользователя...");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }

                Logger.Info($"Проверка наличия в БД логина {newProfile.Login}...");
                using (var command = connection.CreateCommand())
                {
                    var profile = GetByLogin(newProfile.Login);
                    if (profile != null)
                    {
                        if (profile.Login.Equals(newProfile.Login))
                        {
                            Logger.Error("Профиль с таким логином уже существует.");
                            throw new Exception();
                        }
                        if (newProfile.Id.Equals(Guid.Empty))
                        {
                            newProfile.Id = Guid.NewGuid();
                            Logger.Info($"Создание нового ИД для пользователя...");
                        }
                    }

                    newProfile.LastQueryDate = DateTime.Now;
                    newProfile.IsOnline = false;

                    command.CommandText =
                        "INSERT INTO Profiles (Id, Login, Password, Name, Surname, Avatar, IsOnline, LastQueryDate)" +
                        "VALUES (@Id, @Login, @Password, @Name, @Surname, @Avatar, @IsOnline, @LastQueryDate)";
                    command.Parameters.AddWithValue("@Id", newProfile.Id);
                    command.Parameters.AddWithValue("@Login", newProfile.Login);
                    command.Parameters.AddWithValue("@Password", newProfile.Password);
                    command.Parameters.AddWithValue("@Name", newProfile.Name);
                    command.Parameters.AddWithValue("@Surname", newProfile.Surname);
                    command.Parameters.AddWithValue("@Avatar", newProfile.Avatar);
                    command.Parameters.AddWithValue("@IsOnline", "0");
                    command.Parameters.AddWithValue("@LastQueryDate", DateTime.Now);
                    Logger.Info("Попытка создания нового профиля...");
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error($"Неверный аргумент передан, {exception.Message}");
                        throw;
                    }
                    Logger.Info($"Профиль с логином {newProfile.Login} создан.");
                    return newProfile;
                }
            }
        }

        /// <inheritdoc />
        public Profile ChangeProfileData(Profile newData)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug($"Изменение данных о пользователе с ИД {newData.Id}.");
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
                    command.CommandText = "Update Profiles SET Login = @Login, Password = @Password, Name = @Name," +
                                          " Surname = @Surname WHERE Id = @ProfileId";
                    command.Parameters.AddWithValue("@ProfileId", newData.Id);
                    command.Parameters.AddWithValue("@Login", newData.Login);
                    command.Parameters.AddWithValue("@Password", newData.Password);
                    command.Parameters.AddWithValue("@Name", newData.Name);
                    command.Parameters.AddWithValue("@Surname", newData.Surname);
                    Logger.Info("Обновление данных профиля...");
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
            return newData;
        }

        /// <inheritdoc />
        public Profile GetProfile(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение профиля...");
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
                    command.CommandText = "SELECT * FROM Profiles WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    Logger.Info($"Получение профиля с параметрами: ИД = {id}...");
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                return new Profile
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    Login = reader.GetString(reader.GetOrdinal("Login")),
                                    Password = 0.ToString(),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                    Avatar = reader.GetGuid(reader.GetOrdinal("Avatar")),
                                    LastQueryDate = reader.GetDateTime(reader.GetOrdinal("LastQueryDate")),
                                    IsOnline = reader.GetBoolean(reader.GetOrdinal("IsOnline"))
                                };
                            Logger.Error($"Пользователь с id {id} не найден");
                            return null;
                        }
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
        public void DeleteProfile(Guid id)
        {
            Logger.Debug("Удаление пользователя.");
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Info($"Удаление пользователя с id {id}");
                try
                {
                    connection.Open();
                }
                catch (SqlException exception)
                {
                    Logger.Error($"Не могу подключиться к БД, {exception.Message}");
                    throw;
                }
                /*
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM ChatMembers WHERE ProfileId = @Id";
                    logger.Info("Удаление пользователя из чатов.");
                    command.Parameters.AddWithValue("@Id", id);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }*/
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "UPDATE Profiles SET Login = @Login, Password = @Password WHERE Id = @ProfileId";
                    command.Parameters.AddWithValue("@ProfileId", id);
                    command.Parameters.AddWithValue("@Login", "");
                    command.Parameters.AddWithValue("@Password", "");
                    Logger.Info("Удаление пользователя из профилей...");
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
                Logger.Info($"Профиль удален.");
            }
        }

        /// <inheritdoc />
        public IEnumerable<Chat> GetProfileChats(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение пользовательских чатов.");
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
                        "SELECT chat.* FROM Chats chat JOIN ChatMembers member ON chat.ChatId = member.ChatId " +
                        "WHERE member.ProfileId = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    Logger.Info($"Получение чатов пользователя с ИД {id}...");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chatId = reader.GetGuid(reader.GetOrdinal("ChatId"));
                            yield return new Chat
                            {
                                ChatId = chatId,
                                ChatName = reader.GetString(reader.GetOrdinal("ChatName")),
                                ChatMembers = _chatsRepository.GetChatMembers(chatId)
                            };
                        }
                    }
                }
            }
            Logger.Info($"Чаты переданы.");
        }

        /// <inheritdoc />
        public IEnumerable<Profile> FindProfiles(IEnumerable<string> tokens)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Поиск профилей.");
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
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            "SELECT * FROM Profiles WHERE Login Like @Name OR Name Like @Name OR Surname Like @Name";
                        command.Parameters.AddWithValue("@Name", "%" + token + "%");
                        Logger.Info($"Поиск пользователя по строке {token}...");
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
                        {
                            yield return new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetGuid(reader.GetOrdinal("Avatar")),
                                LastQueryDate = reader.GetDateTime(reader.GetOrdinal("LastQueryDate")),
                                IsOnline = reader.GetBoolean(reader.GetOrdinal("IsOnline"))
                            };
                            Logger.Info($"Возвращен пользователь по строке {token}");
                        }
                        reader.Close();
                    }
            }
        }

        /// <inheritdoc />
        public Profile GetProfile(string login, string password, bool isOnline)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение профиля...");
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
                    command.CommandText = "SELECT * FROM Profiles WHERE Login = @Login AND Password = @Password";
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    Logger.Info($"Получение пользователя по логину {login} и паролю {password}...");
                    Profile logined;
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new Exception("Пользователь с данными логином и паролем не найден");

                            logined = new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = 0.ToString(),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetGuid(reader.GetOrdinal("Avatar")),
                                LastQueryDate = reader.GetDateTime(reader.GetOrdinal("LastQueryDate")),
                                IsOnline = reader.GetBoolean(reader.GetOrdinal("IsOnline"))
                            };
                        }
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                    LoginProfile(logined.Id);
                    logined.IsOnline = true;
                    logined.LastQueryDate = DateTime.Now;
                    return logined;
                }
            }
        }

        /// <inheritdoc />
        public void LoginProfile(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Info("Вход в профиль...");
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
                        "UPDATE Profiles SET IsOnline = @isOnline, LastQueryDate = @Date where Id = @Id";
                    command.Parameters.AddWithValue("@isOnline", true);
                    command.Parameters.AddWithValue("@Date", DateTime.Now);
                    command.Parameters.AddWithValue("@Id", id);
                    Logger.Info("Устанавливаем флаг \"в сети\" и дату последнего запроса.");
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
        public void LogoutProfile(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Info("Выход из профиля...");
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
                        "UPDATE Profiles SET IsOnline = @isOnline, LastQueryDate = @Date where Id = @Id";
                    command.Parameters.AddWithValue("@isOnline", false);
                    command.Parameters.AddWithValue("@Date", DateTime.Now);
                    command.Parameters.AddWithValue("@Id", id);
                    Logger.Info("Сбрасываем флаг \"в сети\" и устанавливаем дату последнего запроса.");
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
        public Profile GetByLogin(string login)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Logger.Debug("Получение профиля...");
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
                    command.CommandText = "SELECT * FROM Profiles WHERE Login = @Login";
                    command.Parameters.AddWithValue("@Login", login);
                    Logger.Info($"Получение пользователя по логину {login}");
                    Profile logined;
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;
                            logined = new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetGuid(reader.GetOrdinal("Avatar")),
                                LastQueryDate = reader.GetDateTime(reader.GetOrdinal("LastQueryDate")),
                                IsOnline = reader.GetBoolean(reader.GetOrdinal("IsOnline"))
                            };
                        }
                    }
                    catch (SqlException exception)
                    {
                        Logger.Error(exception.Message);
                        throw;
                    }
                    LoginProfile(logined.Id);
                    logined.IsOnline = true;
                    logined.LastQueryDate = DateTime.Now;
                    return logined;
                }
            }
        }
    }
}