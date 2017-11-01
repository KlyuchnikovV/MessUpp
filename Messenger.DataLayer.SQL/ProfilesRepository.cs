﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;
using System.Data.SqlClient;
using NLog;
using System.Diagnostics;

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
                    return null;
                }
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Profiles (Id, Login, Password, Name, Surname, Avatar) values (@Id, @Login, @Password, @Name, @Surname, @Avatar)";
                    command.Parameters.AddWithValue("@Id", newProfile.Id);
                    command.Parameters.AddWithValue("@Login", newProfile.Login);
                    command.Parameters.AddWithValue("@Password", newProfile.Password);
                    command.Parameters.AddWithValue("@Name", newProfile.Name);
                    command.Parameters.AddWithValue("@Surname", newProfile.Surname);
                    command.Parameters.AddWithValue("@Avatar", newProfile.Avatar);

                    logger.Info("Попытка создания нового профиля с параметрами: ИД = {0}, Логин = {1}, Пароль = {2}, Имя = {3}, Фамилия = {4}, Аватар = {5}.",
                        newProfile.Id, newProfile.Login, newProfile.Password, newProfile.Name, newProfile.Surname, newProfile.Avatar);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch(SqlException exc)
                    {
                        logger.Error("Профиль либо существует либо неверный аргумент передан, {0}", exc.Message);
                        return null;
                    }
                    return newProfile;
                }
            }
        }

        public Profile ChangeProfileData(Profile newData)
        {
            logger.Debug("Изменение данных о пользователе.");
            DeleteProfile(newData.Id);
            return CreateProfile(newData); 
        }

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
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    return null;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Получение профиля с параметрами: ИД = {0}", id);
                    command.CommandText = "SELECT TOP(1) Id, Login, Password, Name, Surname, Avatar FROM Profiles WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                logger.Error("Пользователь с id {0} не найден", id);
                                return null;
                            }

                            return new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetSqlBinary(reader.GetOrdinal("Avatar")).Value,
                            };
                        }
                    }
                    catch(SqlException exception)
                    {
                        logger.Error(exception.Message);
                        return null;
                    }
                }
            }
        }

        public void DeleteProfile(Guid id)
        {
            logger.Debug("Удаление пользователя.");
            using (var connection = new SqlConnection(connectionString))
            {
                logger.Info("Удаление пользователя с id {0}", id);
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
                    logger.Info("Удаление пользователя из чатов.");
                    command.CommandText = "DELETE FROM ChatMembers WHERE ProfileId = @Id";
                    command.Parameters.AddWithValue("@Id", id);
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
                    logger.Info("Удаление пользователя из профилей.");
                    command.CommandText = "DELETE FROM Profiles WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
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


        // Additional methods

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
                    yield break;
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
        }

        public IEnumerable<Profile> GetProfiles(string name, string surname)
        {
            logger.Debug("Получение профилей.");
            using (var connection = new SqlConnection(connectionString))
            {
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
                    if(!surname.Equals(null))
                    {
                        logger.Info("Поиск пользователя по фамилиии");
                        command.CommandText = "SELECT Id, Login, Password, Name, Surname, Avatar FROM Profiles WHERE Surname = @Surname";
                        command.Parameters.AddWithValue("@Surname", surname);
                        SqlDataReader reader;
                        try
                        {
                            reader = command.ExecuteReader();
                        }
                        catch(SqlException exception)
                        {
                            logger.Error(exception.Message);
                            yield break;
                        }
                        if (!reader.Read())
                            throw new ArgumentException($"Пользователь с фамилией {surname} не найден");
                        while (reader.Read())
                        {
                            yield return new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetSqlBinary(reader.GetOrdinal("Avatar")).Value,
                            };
                        }
                    }
                    else if(!name.Equals(null))
                    {
                        logger.Info("Поиск пользователя по имени");
                        command.CommandText = "SELECT Id, Login, Password, Name, Surname, Avatar FROM Profiles WHERE Name = @Name";
                        command.Parameters.AddWithValue("@Name", name);
                        SqlDataReader reader;
                        try
                        {
                            reader = command.ExecuteReader();
                        }
                        catch (SqlException exception)
                        {
                            logger.Error(exception.Message);
                            yield break;
                        }
                        if (!reader.Read())
                            throw new ArgumentException($"Пользователь с именем {name} не найден");
                        while (reader.Read())
                        {
                            yield return new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetSqlBinary(reader.GetOrdinal("Avatar")).Value,
                            };
                        }
                    }
                    else
                    {
                        logger.Error("Задан пустой запрос");
                        yield break;
                    }
                }
            }
        }

        public Profile GetProfile(string login)
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
                    logger.Error("Не могу подключиться к БД, {0}", exception.Message);
                    return null;
                }
                using (var command = connection.CreateCommand())
                {
                    logger.Info("Получение пользователя по логину");
                    command.CommandText = "SELECT TOP(1) Id, Login, Password, Name, Surname, Avatar FROM Profiles WHERE Login = @Login";
                    command.Parameters.AddWithValue("@Login", login);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                throw new ArgumentException($"Пользователь с login {login} не найден");

                            return new Profile
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Login = reader.GetString(reader.GetOrdinal("Login")),
                                Password = reader.GetString(reader.GetOrdinal("Password")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                Avatar = reader.GetSqlBinary(reader.GetOrdinal("Avatar")).Value,
                            };
                        }
                    }
                    catch (SqlException exception)
                    {
                        logger.Error(exception.Message);
                        return null;
                    }
                }
            }
        }
    }
}
