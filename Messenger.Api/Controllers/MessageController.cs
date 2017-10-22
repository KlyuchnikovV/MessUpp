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
    public class MessageController : ApiController
    {
        private readonly IProfilesRepository profilesRepository;
        private readonly IMessagesRepository messagesRepository;
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";

        public MessageController()
        {
            profilesRepository = new ProfilesRepository(ConnectionString);
            messagesRepository = new MessagesRepository(ConnectionString);
        }

        [HttpGet]
        [Route("api/message/{id}")]
        public Message Get(Guid id)
        {
            return messagesRepository.GetMessage(id);
        }

        [HttpPost]
        [Route("api/message")]
        public Message Create([FromBody] Message message)
        {
            return messagesRepository.SendMessage(message);
        }

        [HttpDelete]
        [Route("api/message/{id}")]
        public void Delete(Guid id)
        {
            messagesRepository.DeleteMessage(id);
        }

        // TODO: add
        
    }
}
