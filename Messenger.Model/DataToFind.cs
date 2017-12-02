using System;

namespace Messenger.Model
{
    /// <summary>
    ///     Модель для передачи значений для поиска среди таблиц.
    /// </summary>
    public class DataToFind
    {
        /// <summary>
        ///     Массив слов для поиска.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает массив строковых значений.
        /// </value>
        public string[] Tokens { get; set; }

        /// <summary>
        ///     Уникальный идентификатор пользователя, сделавшего запрос.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid ProfileId { get; set; }
    }
}