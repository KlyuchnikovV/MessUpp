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
    public class ProfileController : ApiController
    {
        private readonly IProfilesRepository profilesRepository;
        /*private const string ConnectionString = @"Data Source = ACER;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";*/
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";

        public ProfileController()
        {
            profilesRepository = new ProfilesRepository(ConnectionString);
        }

        [HttpPost]
        [Route("api/profile")]
        public Profile CreateProfile([FromBody] Profile profile)
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
            catch(Exception exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        [Route("api/profile/{id}")]
        public Profile GetProfile(Guid id)
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
        public void DeleteProfile(Guid id)
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
        public IEnumerable<Profile> FindProfiles([FromBody]DataToFind data)
        {
            try
            { 
                return profilesRepository.FindProfiles(data.tokens).Distinct();
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
        [Route("api/profile/login")]
        public Profile Login([FromBody] Profile profile)
        {
            try
            {
                return profilesRepository.GetProfile(profile.Login, profile.Password, true);
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
                var response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

        [HttpGet]
        [Route("api/profile/logout/{id}")]
        public void Logout(Guid id)
        {
            try
            {
                profilesRepository.LogoutProfile(id);
            }
            catch (SqlException exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
            catch (Exception exception)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NonAuthoritativeInformation)
                {
                    Content = new StringContent(exception.Message)
                };
                throw new HttpResponseException(response);
            }
        }

    }
}
