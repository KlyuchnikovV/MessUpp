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
    // Move to another file and expand. //
    public class FindArray
    {
        public String[] names { get; set; }
        public Guid profileId { get; set; }
    }

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

        [HttpPost]
        [Route("api/profile")]
        public Profile Create([FromBody] Profile profile)
        {
            try
            {
                return profilesRepository.CreateProfile(profile);
            }
            catch(SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
            catch(Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("Профиль с таким логином уже существует.")
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        [Route("api/profile/{id}")]
        public Profile Get(Guid id)
        {
            try
            {
                return profilesRepository.GetProfile(id);
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

        [HttpDelete]
        [Route("api/profile/{id}")]
        public void Delete(Guid id)
        {
            try
            {
                profilesRepository.DeleteProfile(id);
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
        [Route("api/profile/{id}/chats")]
        public IEnumerable<Chat> GetProfileChats(Guid id)
        {
            try
            {
                return profilesRepository.GetProfileChats(id).ToList();
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
        [Route("api/profile/find/profiles")]
        public IEnumerable<Profile> FindProfiles([FromBody]FindArray names)
        {
            try
            { 
                return profilesRepository.FindProfiles(names.names).Distinct();
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

        /* Deprecated
        [HttpGet]
        [Route("api/profile/find/{name}")]
        public IEnumerable<Profile> GetChats(string name)
        {
            if (!name.Equals(null))
                return profilesRepository.FindProfiles( new string[] { name } );
            else
                return null;
        }*/

        [HttpPost]
        [Route("api/profile/login")]
        public Profile GetProfile([FromBody] Profile profile)
        {
            try
            {
                return profilesRepository.GetProfile(profile.Login, profile.Password);
            }
            catch(SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
            catch(Exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation)
                {
                    Content = new StringContent("Пользователь с данными логином и паролем не найден")
                };
                throw new HttpResponseException(response);
            }
        }
        
    }
}
