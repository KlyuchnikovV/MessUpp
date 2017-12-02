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
    ///     Реализация контроллера для чатов.
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class ChatController : ApiController
    {
        private readonly IChatsRepository _chatsRepository;
        private readonly IProfilesRepository _profilesRepository;

        /// <summary>
        ///     Конструктор методов работы с чатами.
        /// </summary>
        [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
        public ChatController()
        {
            _profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            _chatsRepository = new ChatsRepository(Constants.Constants.ConnectionString, _profilesRepository);
        }

        /// <summary>
        ///     Запрос на создание чата.
        /// </summary>
        /// <param name="chat">Данные создаваемого чата.</param>
        /// <returns>Данные созданного чата.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/chat")]
        public Chat CreateChat([FromBody] Chat chat)
        {
            try
            {
                return _chatsRepository.CreateChat(chat);
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
        ///     Запрос на получение чата.
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        /// <returns>Данные найденного чата.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/chat/{id}")]
        public Chat GetChat(Guid id)
        {
            try
            {
                return _chatsRepository.GetChat(id);
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
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на удаление чата.
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpDelete]
        [Route("api/chat/{id}")]
        public void DeleteChat(Guid id)
        {
            try
            {
                _chatsRepository.DeleteChat(id);
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
        ///     Запроса на добавление пользователя к чату.
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        /// <param name="personId">Идентификатор пользователя.</param>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/chat/{id}/add/profile/{personId}")]
        public void AddChatMember(Guid id, Guid personId)
        {
            try
            {
                _chatsRepository.AddChatMember(personId, id);
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
        ///     Запрос на удаление пользователя из чата.
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        /// <param name="personId">Идентификатор пользователя.</param>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpDelete]
        [Route("api/chat/{id}/delete/profile/{personId}")]
        public void DeleteChatMember(Guid id, Guid personId)
        {
            try
            {
                _chatsRepository.DeleteChatMember(personId, id);
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
        ///     Запрос на получение пользователей чата.
        /// </summary>
        /// <param name="id">Идентификатор чата.</param>
        /// <returns>Список профилей чата.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/chat/{id}/get/profiles")]
        public IEnumerable<Profile> GetChatMembers(Guid id)
        {
            try
            {
                var list = _chatsRepository.GetChatMembers(id).ToList();
                var profiles = new List<Profile>();
                foreach (var profileId in list)
                {
                    var profile = _profilesRepository.GetProfile(profileId);
                    profiles.Add(profile);
                    if ((DateTime.Now.TimeOfDay - profile.LastQueryDate.TimeOfDay).Minutes < 2 ||
                        !profile.IsOnline.Equals(true)) continue;
                    _profilesRepository.LogoutProfile(profile.Id);
                    profile.IsOnline = false;
                }
                return profiles;
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
        ///     Запрос на поиск чатов по набору токенов.
        /// </summary>
        /// <param name="data">Набор токенов.</param>
        /// <returns>Список найденных чатов.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/chat/find/chats")]
        public IEnumerable<Chat> FindChats([FromBody] DataToFind data)
        {
            try
            {
                return _chatsRepository.FindChats(data.Tokens, data.ProfileId);
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