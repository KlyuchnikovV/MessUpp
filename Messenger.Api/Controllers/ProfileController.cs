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
    public class ProfileController : ApiController
    {
        private readonly IProfilesRepository profilesRepository;
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";
        public ProfileController()
        {
            profilesRepository = new ProfilesRepository(ConnectionString);
        }

        [HttpGet]
        [Route("api/profile/{id}")]
        public Profile Get(Guid id)
        {
            return profilesRepository.GetProfile(id);
        }

        [HttpPost]
        [Route("api/profile")]
        public Profile Create([FromBody] Profile profile)
        {
            return profilesRepository.CreateProfile(profile);
        }

        [HttpDelete]
        [Route("api/profile/{id}")]
        public void Delete(Guid id)
        {
            profilesRepository.DeleteProfile(id);
        }


        // Additional methods api.
        [HttpGet]
        [Route("api/profile/{id}/chats")]
        public IEnumerable<Chat> GetChats(Guid id)
        {
            return profilesRepository.GetProfileChats(id);
        }

        [HttpGet]
        [Route("api/profile/{name}:{surname}")]
        public IEnumerable<Profile> GetChats(string name, string surname)
        {
            if (!name.Equals(null) || !surname.Equals(null))
                return profilesRepository.GetProfiles(name, surname);
            else
                return null;
        }

        [HttpGet]
        [Route("api/profile/{login}")]
        public Profile GetProfile(string login)
        {
            return profilesRepository.GetProfile(login);
        }

    }
}
