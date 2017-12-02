using System;
using System.Collections.Generic;
using Messenger.Model;

namespace Messenger.DataLayer
{
    /// <summary>
    ///     Интерфейс для методов работы с таблицей "Сообщения".
    /// </summary>
    public interface IMessagesRepository
    {
        /// <summary>
        ///     Создает новое сообщение в чате.
        /// </summary>
        /// <param name="message">Сообщение для добавления в таблицу.</param>
        /// <returns>Созданное сообщение с идентификатором</returns>
        Message CreateMessage(Message message);

        /// <summary>
        ///     Получает сообщение по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сообщения.</param>
        /// <returns>Найденное сообщение.</returns>
        Message GetMessage(Guid id);

        /// <summary>
        ///     Удаляет сообщение по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сообщения.</param>
        void DeleteMessage(Guid id);

        /// <summary>
        ///     Получает все сообщения чата по идентификатору чата.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <returns>Список сообщений.</returns>
        IEnumerable<Message> GetMessages(Guid chatId);

        /// <summary>
        ///     Подсчитывает количество сообщений в чате.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <returns>Количество сообщений чата.</returns>
        int CountMessages(Guid chatId);

        /// <summary>
        ///     Подсчитывает количество прочитанных сообщений пользователя в чате.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <param name="personId">Идентификатор пользователя.</param>
        /// <returns>Количество прочитанных сообщений данного пользователя в чате.</returns>
        int CountReadMessages(Guid chatId, Guid personId);

        /// <summary>
        ///     Поиск среди сообщений, принадлежащих чатам пользователя.
        /// </summary>
        /// <param name="names">Список слов, которые надо найти.</param>
        /// <param name="profileId">Идентификатор пользователя, сделавшего запрос.</param>
        /// <returns>Список сообщений, найденных по данным словам.</returns>
        IEnumerable<Message> FindMessages(IEnumerable<string> names, Guid profileId);

        /// <summary>
        ///     Проверка сообщений, не удаленных автоматически.
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        void CheckUndestroyedMessages(Guid id);

        /// <summary>
        ///     Установка флага прочитанности сообщения.
        /// </summary>
        /// <param name="id">Идентификатор сообщения.</param>
        void UpdateMessageRead(Guid id);

        /// <summary>
        ///     Запуск самоудаления чата.
        /// </summary>
        /// <param name="message">Сообщение к удалению.</param>
        void Destroy(Message message);
    }
}