using System;

namespace Messenger.Model
{
    /// <summary>
    ///     Модель, отображающая строку таблицы "Вложения".
    /// </summary>
    public class Attachment
    {
        /// <summary>
        ///     Уникальный идентификатор вложения.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid AttachId { get; set; }

        /// <summary>
        ///     Значение вложения.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string Data { get; set; }

        /// <summary>
        ///     Тип вложения.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string Type { get; set; }
    }
}