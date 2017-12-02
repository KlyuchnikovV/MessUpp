using System;
using System.Collections.Generic;

namespace Messenger.Model
{
    /// <summary>
    ///     Модель, отображающая строку таблицы "Чаты".
    /// </summary>
    public class Chat
    {
        /// <summary>
        ///     Уникальный идентификатор чата.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid ChatId { get; set; }

        /// <summary>
        ///     Название чата.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string ChatName { get; set; }

        /// <summary>
        ///     Список уникальных идентификаторов пользователей, состоящих в чате.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение списка уникальных идентификаторов.
        /// </value>
        public IEnumerable<Guid> ChatMembers { get; set; }
    }
}