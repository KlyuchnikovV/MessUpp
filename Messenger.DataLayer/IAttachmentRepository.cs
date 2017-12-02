using System;
using Messenger.Model;

namespace Messenger.DataLayer
{
    /// <summary>
    ///     Интерфейс для методов работы с таблицей "Вложения".
    /// </summary>
    public interface IAttachmentRepository
    {
        /// <summary>
        ///     Добавление вложения в таблицу.
        /// </summary>
        /// <param name="file">Данные вложения.</param>
        /// <returns>Данные вложения с идентификатором.</returns>
        Attachment LoadAttachment(Attachment file);

        /// <summary>
        ///     Получение вложения по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор вложения.</param>
        /// <returns>Данные вложения.</returns>
        Attachment GetAttachment(Guid id);

        /// <summary>
        ///     Удаление вложения по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор вложения.</param>
        void DeleteAttachment(Guid id);
    }
}