using System;
using System.Collections.Generic;
using Messenger.Model;

namespace Messenger.DataLayer
{
    /// <summary>
    ///     Интерфейс для методов работы с таблицей "Профили".
    /// </summary>
    public interface IProfilesRepository
    {
        /// <summary>
        ///     Создание нового профиля.
        /// </summary>
        /// <param name="newProfile">Данные нового профиля.</param>
        /// <returns>Созданный профиль с уникальным идентификатором нового профиля.</returns>
        Profile CreateProfile(Profile newProfile);

        /// <summary>
        ///     Возвращает профиль пользователя с заданным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор профиля, данные которого надо найти.</param>
        /// <returns>Найденный профиль.</returns>
        Profile GetProfile(Guid id);

        /// <summary>
        ///     Переписывает информацию о профиле.
        /// </summary>
        /// <param name="newData">Данные переписымаего профиля.</param>
        /// <returns>Переписанный профиль.</returns>
        Profile ChangeProfileData(Profile newData);

        /// <summary>
        ///     Удаляет профиль с данным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор профиля, данные которого надо удалить.</param>
        void DeleteProfile(Guid id);

        /// <summary>
        ///     Возвращает список чатов, в которых состоит пользователь с данным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор профиля, чаты которого надо выдать.</param>
        /// <returns>Список чатов, в которых состоит пользователь с данным идентификатором.</returns>
        IEnumerable<Chat> GetProfileChats(Guid id);

        /// <summary>
        ///     Возвращает список профилей с данными именем, фамилией или логином.
        /// </summary>
        /// <param name="tokens">Массив строк, которые должны быть найдены.</param>
        /// <returns>Список профилей с заданными именем, фамилией или логином.</returns>
        IEnumerable<Profile> FindProfiles(IEnumerable<string> tokens);

        /// <summary>
        ///     Возвращает профиль с данными логином и паролем, используется для входа.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="isOnline">Установить ли флаг присутствия пользователя в сети.</param>
        /// <returns>Найденный профиль.</returns>
        Profile GetProfile(string login, string password, bool isOnline);

        /// <summary>
        ///     Устанавливает флаг присутствия и дату последнего запроса.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        void LoginProfile(Guid id);

        /// <summary>
        ///     Сбрасывает флаг присутствия и дату последнего запроса.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        void LogoutProfile(Guid id);

        /// <summary>
        ///     Поиск пользователя только по логину.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <returns>Найденный профиль.</returns>
        Profile GetByLogin(string login);
    }
}