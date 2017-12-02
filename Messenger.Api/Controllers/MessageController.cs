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

        public MessageController()
        {
            profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            messagesRepository = new MessagesRepository(Constants.Constants.ConnectionString);
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
        [Route("api/message/chat/{id}/profile/{profileId}")]
        public IEnumerable<Message> GetMessages(Guid id, Guid profileId)
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
                foreach(var message in list)
                {
                    if(!message.IsRead && profileId != message.ProfileId)
                    {
                        messagesRepository.UpdateMessageRead(message.MessageId);
                    }
                    if(profileId != message.ProfileId)
                        messagesRepository.Destroy(message);
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

        [HttpGet]
        [Route("api/message/chat/{id}/profile/{profileId}/noread")]
        public Message GetMessagesWithoutReadFlag(Guid id, Guid profileId)
        {
            try
            {
                messagesRepository.CheckUndestroyedMessages(id);
                List<Message> list = messagesRepository.GetMessages(id).ToList();
                list.Sort(delegate (Message one, Message two)
                {
                    return one.Date.CompareTo(two.Date);
                }
                );
                if ((DateTime.Now.TimeOfDay - list.Last().Date.TimeOfDay).TotalSeconds < 30)
                    return list.Last();
                else
                    return null;
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
        [Route("api/message/chat/{chatId}/profile/{profileId}/count")]
        public int CountMessages(Guid chatId, Guid profileId)
        {
            try
            {
                Profile profile = profilesRepository.GetProfile(profileId);
                if (!profile.IsOnline || ((DateTime.Now.TimeOfDay - profile.LastQueryDate.TimeOfDay).Minutes >= 1))
                    profilesRepository.LoginProfile(profileId);
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

        [HttpGet]
        [Route("api/message/chat/{chatId}/profile/{profileId}/read")]
        public int CountReadMessages(Guid chatId, Guid profileId)
        {
            try
            {
                return messagesRepository.CountReadMessages(chatId, profileId);
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
