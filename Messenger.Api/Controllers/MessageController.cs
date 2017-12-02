using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Messenger.DataLayer;
using Messenger.DataLayer.SQL;
using Messenger.Model;

namespace Messenger.Api.Controllers
{
    /// <summary>
    ///     Реализация контроллера для сообщений.
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class MessageController : ApiController
    {
        private readonly IMessagesRepository _messagesRepository;
        private readonly IProfilesRepository _profilesRepository;

        /// <summary>
        ///     Конструктор методов работы с сообщениями.
        /// </summary>
        [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
        public MessageController()
        {
            _profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            _messagesRepository = new MessagesRepository(Constants.Constants.ConnectionString);
        }

        /// <summary>
        ///     Запрос на создание сообщения.
        /// </summary>
        /// <param name="message">Данные сообщения.</param>
        /// <returns>Созданное сообщение.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/message")]
        public Message CreateMessage([FromBody] Message message)
        {
            try
            {
                return _messagesRepository.CreateMessage(message);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на получение сообщения по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сообщения.</param>
        /// <returns>Сообщение найденное по идентификатору.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/message/{id}")]
        public Message GetMessage(Guid id)
        {
            try
            {
                return _messagesRepository.GetMessage(id);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
            catch (Exception exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на удаление сообщения.
        /// </summary>
        /// <param name="id">Идентификатор сообщения.</param>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpDelete]
        [Route("api/message/{id}")]
        public void DeleteMessage(Guid id)
        {
            try
            {
                _messagesRepository.DeleteMessage(id);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на получение всех сообщений чата.
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Список сообщений чата.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/message/chat/{id}/profile/{profileId}")]
        public IEnumerable<Message> GetMessages(Guid id, Guid profileId)
        {
            try
            {
                _messagesRepository.CheckUndestroyedMessages(id);
                var list = _messagesRepository.GetMessages(id).ToList();
                list.Sort((one, two) => one.Date.CompareTo(two.Date));
                foreach (var message in list)
                {
                    if (!message.IsRead && profileId != message.ProfileId)
                        _messagesRepository.UpdateMessageRead(message.MessageId);
                    if (profileId != message.ProfileId)
                        _messagesRepository.Destroy(message);
                }
                return list;
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на получение последнего сообщения без установки флага "прочитано".
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Последнее сообщение чата.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/message/chat/{id}/profile/{profileId}/noread")]
        public Message GetMessagesWithoutReadFlag(Guid id, Guid profileId)
        {
            try
            {
                _messagesRepository.CheckUndestroyedMessages(id);
                var list = _messagesRepository.GetMessages(id).ToList();
                list.Sort((one, two) => one.Date.CompareTo(two.Date));
                return (DateTime.Now.TimeOfDay - list.Last().Date.TimeOfDay).TotalSeconds < 30 ? list.Last() : null;
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на подсчет всех сообщений чата.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Число всех сообщений.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/message/chat/{chatId}/profile/{profileId}/count")]
        public int CountMessages(Guid chatId, Guid profileId)
        {
            try
            {
                var profile = _profilesRepository.GetProfile(profileId);
                if (!profile.IsOnline || (DateTime.Now.TimeOfDay - profile.LastQueryDate.TimeOfDay).Minutes >= 1)
                    _profilesRepository.LoginProfile(profileId);
                return _messagesRepository.CountMessages(chatId);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на подсчет прочитанных сообщений отправленных пользователем в чат.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <param name="profileId">Идентификатор профиля.</param>
        /// <returns>Число прочитанных сообщений.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/message/chat/{chatId}/profile/{profileId}/read")]
        public int CountReadMessages(Guid chatId, Guid profileId)
        {
            try
            {
                return _messagesRepository.CountReadMessages(chatId, profileId);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на поиск сообщений по набору токенов.
        /// </summary>
        /// <param name="data">Набор токенов для поиска.</param>
        /// <returns>Список найденных сообщений.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/message/find/messages")]
        public IEnumerable<Message> FindMessages([FromBody] DataToFind data)
        {
            try
            {
                return _messagesRepository.FindMessages(data.Tokens, data.ProfileId);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }
    }
}