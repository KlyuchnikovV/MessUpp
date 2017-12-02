using System;

namespace Messenger.Model
{
    /// <summary>
    ///     Модель, отображающая строку таблицы "Профили".
    /// </summary>
    public class Profile
    {
        /// <summary>
        ///     Уникальный идентификатор пользователя.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        ///     Логин пользователя.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string Login { get; set; }

        /// <summary>
        ///     Пароль пользователя.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        ///     Имя пользователя.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Фамилия пользователя.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает строковое значение.
        /// </value>
        public string Surname { get; set; }

        /// <summary>
        ///     Уникальный идентификатор аватара пользователя(если 0 - то нет аватара).
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение уникального идентификатора.
        /// </value>
        public Guid Avatar { get; set; }

        /// <summary>
        ///     Флаг присутствия пользователя в сети.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает булево значение.
        /// </value>
        public bool IsOnline { get; set; }

        /// <summary>
        ///     Значение даты и времени последнего запроса от пользователя.
        /// </summary>
        /// <value>
        ///     Устанавливает/получает значение даты/времени.
        /// </value>
        public DateTime LastQueryDate { get; set; }
    }
}