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

        [HttpPost]
        [Route("api/chat")]
        public Chat Create([FromBody] Chat chat)
        {
            return chatsRepository.CreateChat(chat);
        }

        [HttpDelete]
        [Route("api/chat/{id}")]
        public void Delete(Guid id)
        {
            chatsRepository.DeleteChat(id);
        }


        // Additional methods.
        [HttpGet]
        [Route("api/chat/{name}")]
        public IEnumerable<Chat> GetChat(string name)
        {
            return chatsRepository.GetChat(name);
        }
    }
}
