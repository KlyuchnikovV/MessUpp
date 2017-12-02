using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Messenger.DataLayer;
using Messenger.DataLayer.SQL;
using Messenger.Model;

namespace Messenger.Api.Controllers
{
    /// <summary>
    ///     Реализация контроллера для вложений.
    /// </summary>
    [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
    public class AttachmentController : ApiController
    {
        private readonly IAttachmentRepository _attachRepository;

        /// <summary>
        ///     Конструктор методов работы с вложениями.
        /// </summary>
        [SuppressMessage("ReSharper", "InheritdocConsiderUsage")]
        public AttachmentController()
        {
            _attachRepository = new AttachmentRepository(Constants.Constants.ConnectionString);
        }

        /// <summary>
        ///     Запрос на создание вложения.
        /// </summary>
        /// <param name="attach">Данные вложения.</param>
        /// <returns>Данные вложения с идентификатором.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpPost]
        [Route("api/attach")]
        public Attachment CreateAttachment([FromBody] Attachment attach)
        {
            try
            {
                return _attachRepository.LoadAttachment(attach);
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
        ///     Запрос на получение вложения по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор вложения.</param>
        /// <returns>Данные вложения.</returns>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpGet]
        [Route("api/attach/{id}")]
        public Attachment GetAttachment(Guid id)
        {
            try
            {
                return _attachRepository.GetAttachment(id);
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
        ///     Запрос на удаление вложения по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор вложения.</param>
        /// <exception cref="HttpResponseException">Ошибка обработки запроса.</exception>
        [HttpDelete]
        [Route("api/attach/{id}")]
        public void DeleteChat(Guid id)
        {
            try
            {
                _attachRepository.DeleteAttachment(id);
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