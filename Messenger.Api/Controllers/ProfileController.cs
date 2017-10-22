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
        // TODO: Add some methods
    }
}
