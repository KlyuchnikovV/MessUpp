using System;
using System.Collections.Generic;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IProfilesRepository
    {
        // Создание нового профиля. //
        Profile CreateProfile(Profile newProfile);

        // Переписывает информацию о пользователе. //
        Profile GetProfile(Guid id);

        // Возвращает профиль пользователя с данным ИД. // 
        Profile ChangeProfileData(Profile newData);

        //  Удаляет профиль пользователя с данным ИД. //
        void DeleteProfile(Guid id);

        // Возвращает коллекцию чатов, в которых состоит пользователь с данным ИД. //
        IEnumerable<Chat> GetProfileChats(Guid id);

        // Возвращает коллекцию профилей с данными именем, фамилией или логином. //
        IEnumerable<Profile> FindProfiles(string[] names);

        // Возвращает профиль с данными логином и паролем, используется для входа. //
        Profile GetProfile(string login, string password, bool isOnline);

        void LoginProfile(Guid id);

        void LogoutProfile(Guid id);

        Profile GetByLogin(string login);
    }
}
