﻿using System;
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
        private const string ConnectionString = @"Data Source = ACER;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";
        /*private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";*/
        public ChatController()
        {
            profilesRepository = new ProfilesRepository(ConnectionString);
            chatsRepository = new ChatsRepository(ConnectionString, profilesRepository);
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
                return chatsRepository.GetChatMembers(id);
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
