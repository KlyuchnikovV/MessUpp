using System;
using System.Collections.Generic;
using Messenger.Model;

namespace Messenger.DataLayer
{
    /// <summary>
    ///     Интерфейс для методов работы с таблицей "Чаты".
    /// </summary>
    public interface IChatsRepository
    {
        /// <summary>
        ///     Создание нового чата.
        /// </summary>
        /// <param name="chat">Данные нового чата.</param>
        /// <returns>Созданный чат с идентификатором.</returns>
        Chat CreateChat(Chat chat);

        /// <summary>
        ///     Получение чата по идентификатору.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <returns>Найденный по идентификатору чат.</returns>
        Chat GetChat(Guid chatId);

        /// <summary>
        ///     Удаление чата по идентификатору.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        void DeleteChat(Guid chatId);

        /// <summary>
        ///     Получение профилей, состоящих в чате.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <returns>Список пользователей чата.</returns>
        IEnumerable<Guid> GetChatMembers(Guid chatId);

        /// <summary>
        ///     Добавление пользователя к чату.
        /// </summary>
        /// <param name="profileId">Идентификатор пользователя.</param>
        /// <param name="chatId">Идентификатор чата.</param>
        void AddChatMember(Guid profileId, Guid chatId);

        /// <summary>
        ///     Удаление пользователя из чата.
        /// </summary>
        /// <param name="profileId">Идентификатор пользователя.</param>
        /// <param name="chatId">Идентификатор чата.</param>
        void DeleteChatMember(Guid profileId, Guid chatId);

        /// <summary>
        ///     Возвращает список чатов с данным названием.
        /// </summary>
        /// <param name="names">Список слов, по которым производится поиск.</param>
        /// <param name="profileId">Идентификатор пользователя.</param>
        /// <returns>Список найденных чатов.</returns>
        IEnumerable<Chat> FindChats(IEnumerable<string> names, Guid profileId);
    }
}