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
    ///     Реализация контроллера для профилей.
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class ProfileController : ApiController
    {
        private readonly IProfilesRepository _profilesRepository;

        /// <summary>
        ///     Конструктор методов работы с профилями.
        /// </summary>
        public ProfileController()
        {
            _profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
        }

        /// <summary>
        ///     Запрос на создание профиля.
        /// </summary>
        /// <param name="profile">Данные создаваемого профиля.</param>
        /// <returns>Созданный профиль.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/profile")]
        public Profile CreateProfile([FromBody] Profile profile)
        {
            try
            {
                return _profilesRepository.CreateProfile(profile);
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
        ///     Запрос на получение профиля по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Профиль найденный по идентификатору.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/profile/{id}")]
        public Profile GetProfile(Guid id)
        {
            try
            {
                return _profilesRepository.GetProfile(id);
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
        ///     Запрос на обновление данных профиля.
        /// </summary>
        /// <param name="profile">Данные профиля для обновления.</param>
        /// <returns>Новые данные профиля.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/profile/update")]
        public Profile UpdateProfile([FromBody] Profile profile)
        {
            try
            {
                return _profilesRepository.ChangeProfileData(profile);
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
        ///     Запрос на удаление профиля по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpDelete]
        [Route("api/profile/{id}")]
        public void DeleteProfile(Guid id)
        {
            try
            {
                _profilesRepository.DeleteProfile(id);
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
        ///     Запрос на получение чатов пользователя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <returns>Список чатов пользователя.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/profile/{id}/chats")]
        public IEnumerable<Chat> GetProfileChats(Guid id)
        {
            try
            {
                return _profilesRepository.GetProfileChats(id).ToList();
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
        ///     Запрос на поиск пользователя по токенам.
        /// </summary>
        /// <param name="data">Список токенов для поиска и идентификатор пользователя, сделавшего запрос.</param>
        /// <returns>Список найденных профилей.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/profile/find/profiles")]
        public IEnumerable<Profile> FindProfiles([FromBody] DataToFind data)
        {
            try
            {
                return _profilesRepository.FindProfiles(data.Tokens).Distinct();
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
        ///     Запрос на поиск профиля по логину и паролю.
        /// </summary>
        /// <param name="profile">Профиль с логиноми паролем.</param>
        /// <returns>Найденый по логину и паролю профиль.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/profile/login")]
        public Profile Login([FromBody] Profile profile)
        {
            try
            {
                return _profilesRepository.GetProfile(profile.Login, profile.Password, true);
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
                var response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на сброс флага "в сети".
        /// </summary>
        /// <param name="id">Идентификатор профиля.</param>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/profile/logout/{id}")]
        public void Logout(Guid id)
        {
            try
            {
                _profilesRepository.LogoutProfile(id);
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
                var response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        /// <summary>
        ///     Запрос на поиск пользователя по логину.
        /// </summary>
        /// <param name="data">Набор токенов(Использоваться будет только первый!).</param>
        /// <returns>Найденныйпо логину профиль.</returns>
        [HttpPost]
        [Route("api/profile/find/login")]
        public Profile GetProfile([FromBody] DataToFind data)
        {
            return _profilesRepository.GetByLogin(data.Tokens[0]);
        }
    }
}