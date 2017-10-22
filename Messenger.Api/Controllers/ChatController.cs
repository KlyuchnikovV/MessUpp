using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Messenger.Model;
using Messenger.DataLayer;
using Messenger.DataLayer.SQL;

namespace Messenger.Api.Controllers
{
    public class ChatController : ApiController
    {
        private readonly IProfilesRepository profilesRepository;
        private readonly IChatsRepository chatsRepository;
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";
        public ChatController()
        {
            profilesRepository = new ProfilesRepository(ConnectionString);
            chatsRepository = new ChatsRepository(ConnectionString, profilesRepository);
        }

        [HttpGet]
        [Route("api/chat/{id}")]
        public Chat Get(Guid id)
        {
            return chatsRepository.GetChat(id);
        }

        // TODO: repair
        [HttpPost]
        [Route("api/chat")]
        public Chat Create([FromBody] Chat chat)
        {
            List<Guid> chatMembers = new List<Guid>();
            foreach(Profile profile in chat.ChatMembers)
            {
                chatMembers.Add(profile.Id);
            }
            return chatsRepository.CreateChat(chatMembers, chat.ChatName);
        }

        [HttpDelete]
        [Route("api/chat/{id}")]
        public void Delete(Guid id)
        {
            chatsRepository.DeleteChat(id);
        }

        // TODO: repair api/chat/{profileid}/chats - looks strange
        [HttpGet]
        [Route("api/chat/{id}/chats")]
        public IEnumerable<Chat> GetChats(Guid id)
        {
            return chatsRepository.GetProfileChats(id);
        }
    }
}
