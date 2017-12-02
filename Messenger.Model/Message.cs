using System;

namespace Messenger.Model
{
    /// <summary>
    ///     Модель, отображающая строку таблицы "Сообщения".
    /// </summary>
    public class Message
    {
        /// <summary>
        ///     Уникальный идентификатор сообщения.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid MessageId { get; set; }

        /// <summary>
        ///     Уникальный идентификатор пользователя.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid ProfileId { get; set; }

        /// <summary>
        ///     Уникальный идентификатор чата.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid ChatId { get; set; }

        /// <summary>
        ///     Текст сообщения.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string MessageText { get; set; }

        /// <summary>
        ///     Значение даты и времени отправки сообщения.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение даты/времени.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Количество секунд до самоудаления сообщения(если 0 то сообщение не должно быть удалено).
        /// </summary>
        /// <value>
        ///     Устанавливает/получает целочисленное значение.
        /// </value>
        public int TimeToDestroy { get; set; }

        /// <summary>
        ///     Уникальный идентификатор вложения.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid Attachment { get; set; }

        /// <summary>
        ///     Флаг того, что сообщение прочитано.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает булево значение.
        /// </value>
        public bool IsRead { get; set; }
    }
}