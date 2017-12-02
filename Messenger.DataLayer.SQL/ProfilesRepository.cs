using System;
using System.Collections.Generic;
using System.Linq;
using Messenger.Model;
using System.Data.SqlClient;
using NLog;

namespace Messenger.DataLayer.SQL
{
    public class ProfilesRepository : IProfilesRepository
    {
        private readonly string connectionString;
        private readonly IChatsRepository chatsRepository;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ProfilesRepository(string _connectionString)
        {
            connectionString = _connectionString;
            chatsRepository = new ChatsRepository(_connectionString, this);
        }

        // Создание нового профиля. //
        public Profile CreateProfile(Profile newProfile)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Создание пользователя...");
                try
                {
                    connection.Open();
                }
                catch(SqlException exception)
                {
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    throw exception;
                }

                logger.Info($"Проверка логина {newProfile.Login}...");

                using (var command = connection.CreateCommand())
                {
                    List<Profile> profiles = FindProfiles( new string[] { newProfile.Login } ).ToList();
                    foreach(var profile in profiles)
                    {
                        if(profile.Login.Equals(newProfile.Login))
                        {
                            logger.Error("Профиль с таким логином уже существует.");
                            throw new Exception();
                        }
                            
                    }
                    if(newProfile.Id.Equals(Guid.Empty))
                    {
                        newProfile.Id = Guid.NewGuid();
                        logger.Info($"Создание нового ИД для пользователя...");
                    }

                    newProfile.LastQueryDate = DateTime.Now;
                    newProfile.IsOnline = false;
                    
                    command.CommandText =
                        "INSERT INTO Profiles (Id, Login, Password, Name, Surname, Avatar, IsOnline, LastQueryDate) values (@Id, @Login, @Password, @Name, @Surname, @Avatar, @IsOnline, @LastQueryDate)";
                    command.Parameters.AddWithValue("@Id", newProfile.Id);
                    command.Parameters.AddWithValue("@Login", newProfile.Login);
                    command.Parameters.AddWithValue("@Password", newProfile.Password);
                    command.Parameters.AddWithValue("@Name", newProfile.Name);
                    command.Parameters.AddWithValue("@Surname", newProfile.Surname);
                    command.Parameters.AddWithValue("@Avatar", newProfile.Avatar);
                    command.Parameters.AddWithValue("@IsOnline", "0");
                    command.Parameters.AddWithValue("@LastQueryDate", DateTime.Now);

                    logger.Info(
                        "Попытка создания нового профиля с параметрами: ИД = {0}, Логин = {1}, Пароль = {2}, Имя = {3}, Фамилия = {4}, Аватар = {5}.",
                        newProfile.Id, newProfile.Login, newProfile.Password, newProfile.Name, newProfile.Surname, newProfile.Avatar);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch(SqlException exception)
                    {
                        logger.Error($"Неверный аргумент передан, {exception.Message}");
                        throw exception;
                    }
                    logger.Info($"Профиль с логином {newProfile.Login} создан.");
                    return newProfile;
                }
            }
        }

        // Переписывает информацию о пользователе. //
        public Profile ChangeProfileData(Profile newData)
        {
            logger.Debug($"Изменение данных о пользователе с ИД { newData.Id }.");
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
                    logger.Info("Обновление данных профиля.");
                    command.CommandText = "Update Profiles SET Login = @Login, Password = @Password, Name = @Name, Surname = @Surname WHERE Id = @ProfileId";
                    command.Parameters.AddWithValue("@ProfileId", newData.Id);
                    command.Parameters.AddWithValue("@Login", newData.Login);
                    command.Parameters.AddWithValue("@Password", newData.Password);
                    command.Parameters.AddWithValue("@Name", newData.Name);
                    command.Parameters.AddWithValue("@Surname", newData.Surname);
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
            return newData;


        }

        // Возвращает профиль пользователя с данным ИД. // 
        public Profile GetProfile(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Debug("Получение профиля...");
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
                    logger.Info($"Получение профиля с параметрами: ИД = {id}");
                    command.CommandText = "SELECT TOP(1) * FROM Profiles WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                logger.Error($"Пользователь с id {id} не найден");
                                return null;
                            }

                            return new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = 0.ToString(),//reader.GetString(reader.GetOrdinal("Password")), // Не обязателдь передавать пароль. //
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetGuid(reader.GetOrdinal("Avatar")),
                                LastQueryDate = reader.GetDateTime(reader.GetOrdinal("LastQueryDate")),
                                IsOnline = reader.GetBoolean(reader.GetOrdinal("IsOnline"))
                            };
                        }
                    }
                    catch(SqlException exception)
                    {
                        logger.Error(exception.Message);
                        throw exception;
                    }
                }
            }
        }

        //  Удаляет профиль пользователя с данным ИД. //
        public void DeleteProfile(Guid id)
        {
            logger.Debug("Удаление пользователя.");
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Info($"Удаление пользователя с id {id}");
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
                    logger.Info("Удаление пользователя из чатов.");
                    command.CommandText = "DELETE FROM ChatMembers WHERE ProfileId = @Id";
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
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Удаление пользователя из профилей.");
                    command.CommandText = "Update Profiles SET Login = @Login, Password = @Password WHERE Id = @ProfileId";
                    command.Parameters.AddWithValue("@ProfileId", id);
                    command.Parameters.AddWithValue("@Login", "");
                    command.Parameters.AddWithValue("@Password", "");
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
                logger.Info($"Профиль удален.");
            }
        }

        // Возвращает коллекцию чатов, в которых состоит пользователь с данным ИД. //
        public IEnumerable<Chat> GetProfileChats(Guid id)
        {
            logger.Debug("Получение пользовательских чатов.");
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
                    logger.Info("Получение чатов пользователя с id {0}", id);
                    command.CommandText = "SELECT chat.* FROM Chats chat JOIN ChatMembers member ON chat.ChatId = member.ChatId WHERE member.ProfileId = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var chatId = reader.GetGuid(reader.GetOrdinal("ChatId"));
                            yield return new Chat
                            {
                                ChatId = chatId,
                                ChatName = reader.GetString(reader.GetOrdinal("ChatName")),
                                ChatMembers = chatsRepository.GetChatMembers(chatId)
                            };
                        }
                    }
                }
            }
            logger.Info($"Чаты переданы.");
        }

        // Возвращает коллекцию профилей с данными именем, фамилией или логином. //
        public IEnumerable<Profile> FindProfiles(string[] tokens)
        {
            logger.Debug("Поиск профилей.");
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
                    
                        logger.Info($"Поиск пользователя по строке {token}");
                        command.CommandText = "SELECT * FROM Profiles WHERE Login Like @Name OR Name Like @Name OR Surname Like @Name";
                        command.Parameters.AddWithValue("@Name", "%" + token + "%");
                        SqlDataReader reader;
                        try
                        {
                            reader = command.ExecuteReader();
                        }
                        catch(SqlException exception)
                        {
                            logger.Error(exception.Message);
                            throw exception;
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
                            logger.Info($"Возвращен пользователь по строке {token}");
                        }
                        reader.Close();
                    }
                }
            }
        }

        // Возвращает профиль с данными логином и паролем, используется для входа. //
        public Profile GetProfile(string login, string password, bool isOnline)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                Profile logined = null;
                logger.Debug("Получение профиля...");
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
                    logger.Info($"Получение пользователя по логину {login} и паролю {password}");
                    command.CommandText = "SELECT TOP(1) * FROM Profiles WHERE Login = @Login AND Password = @Password";
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new Exception("Пользователь с данными логином и паролем не найден");

                            logined =  new Profile
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
                        logger.Error(exception.Message);
                        throw exception;
                    }
                    LoginProfile(logined.Id);
                    logined.IsOnline = true;
                    logined.LastQueryDate = DateTime.Now;
                    return logined;
                }
            }
        }

        public Profile GetByLogin(string login)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                Profile logined = null;
                logger.Debug("Получение профиля...");
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
                    logger.Info($"Получение пользователя по логину {login}");
                    command.CommandText = "SELECT TOP(1) * FROM Profiles WHERE Login = @Login";
                    command.Parameters.AddWithValue("@Login", login);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;
                            //    throw new Exception("Пользователь с данными логином и паролем не найден");

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
                        logger.Error(exception.Message);
                        throw exception;
                    }
                    LoginProfile(logined.Id);
                    logined.IsOnline = true;
                    logined.LastQueryDate = DateTime.Now;
                    return logined;
                }
            }
        }

        public void LoginProfile(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Info("Вход в профиль...");
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
                    command.CommandText = "UPDATE Profiles SET IsOnline = @isOnline, LastQueryDate = @Date where Id = @Id";
                    command.Parameters.AddWithValue("@isOnline", true);
                    command.Parameters.AddWithValue("@Date", DateTime.Now);
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
                }

            }
        }

        public void LogoutProfile(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Info("Выход из профиля...");
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

                    command.CommandText = "UPDATE Profiles SET IsOnline = @isOnline, LastQueryDate = @Date where Id = @Id";
                    command.Parameters.AddWithValue("@isOnline", false);
                    command.Parameters.AddWithValue("@Date", DateTime.Now);
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

                }
            }
        }
    }
}
