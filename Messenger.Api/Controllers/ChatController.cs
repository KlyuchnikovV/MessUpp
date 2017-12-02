using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Messenger.Model;
using Messenger.DataLayer;
using Messenger.DataLayer.SQL;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net;

namespace Messenger.Api.Controllers
{
    public class ChatController : ApiController
    {
        private readonly IProfilesRepository profilesRepository;
        private readonly IChatsRepository chatsRepository;

        public ChatController()
        {
            profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            chatsRepository = new ChatsRepository(Constants.Constants.ConnectionString, profilesRepository);
        }

        [HttpPost]
        [Route("api/chat")]
        public Chat CreateChat([FromBody] Chat chat)
        {
            try
            {
                return chatsRepository.CreateChat(chat);
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
        [Route("api/chat/{id}")]
        public Chat GetChat(Guid id)
        {
            try
            {
                return chatsRepository.GetChat(id);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
            catch(Exception exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpDelete]
        [Route("api/chat/{id}")]
        public void DeleteChat(Guid id)
        {
            try
            {
                chatsRepository.DeleteChat(id);
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
        [Route("api/chat/{id}/add/profile/{personId}")]
        public void AddChatMember(Guid id, Guid personId)
        {
            try
            {
                chatsRepository.AddChatMember(personId, id);
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

        [HttpDelete]
        [Route("api/chat/{id}/delete/profile/{personId}")]
        public void DeleteChatMember(Guid id, Guid personId)
        {
            try
            {
                chatsRepository.DeleteChatMember(personId, id);
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
        [Route("api/chat/{id}/get/profiles")]
        public IEnumerable<Profile> GetChatMembers(Guid id)
        {
            try
            {
                List<Profile> list = chatsRepository.GetChatMembers(id).ToList();
                foreach(var profile in list)
                {
                    if (((DateTime.Now.TimeOfDay - profile.LastQueryDate.TimeOfDay).Minutes >= 2) && profile.IsOnline.Equals(true))
                    {
                        profilesRepository.LogoutProfile(profile.Id);
                        profile.IsOnline = false;
                    }
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

        [HttpPost]
        [Route("api/chat/find/chats")]
        public IEnumerable<Chat> FindChats([FromBody]DataToFind data)
        {
            try
            {
                return chatsRepository.FindChats(data.tokens, data.profileId);
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
