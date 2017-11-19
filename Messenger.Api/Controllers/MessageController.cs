using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Messenger.Model;
using Messenger.DataLayer;
using Messenger.DataLayer.SQL;
using System.Data.SqlClient;

namespace Messenger.Api.Controllers
{
    public class MessageController : ApiController
    {
        private readonly IProfilesRepository profilesRepository;
        private readonly IMessagesRepository messagesRepository;
        /*private const string ConnectionString = @"Data Source = ACER;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";*/
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";

        public MessageController()
        {
            profilesRepository = new ProfilesRepository(ConnectionString);
            messagesRepository = new MessagesRepository(ConnectionString);
        }

        [HttpPost]
        [Route("api/message")]
        public Message CreateMessage([FromBody] Message message)
        {
            try
            {
                return messagesRepository.SendMessage(message);
            }
            catch(SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        [Route("api/message/{id}")]
        public Message GetMessage(Guid id)
        {
            try
            {
                return messagesRepository.GetMessage(id);
            }
            catch(SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
            catch(Exception exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpDelete]
        [Route("api/message/{id}")]
        public void DeleteMessage(Guid id)
        {
            try
            { 
                messagesRepository.DeleteMessage(id);
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

        [HttpGet]
        [Route("api/message/chat/{id}")]
        public IEnumerable<Message> GetMessages(Guid id)
        {
            try
            {
                messagesRepository.CheckUndestroyedMessages(id);
                List<Message> list =  messagesRepository.GetMessages(id).ToList();
                list.Sort(delegate (Message one, Message two)
                    {
                        return one.Date.CompareTo(two.Date);
                    }
                );
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

        [HttpGet]
        [Route("api/message/chat/{chatId}/count")]
        public int CountMessages(Guid chatId)
        {
            try
            { 
                return messagesRepository.CountMessages(chatId);
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

        [HttpPost]
        [Route("api/message/find/messages")]
        public IEnumerable<Message> FindMessages([FromBody]DataToFind data)
        {
            try
            { 
                return messagesRepository.FindMessages(data.tokens, data.profileId);
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
