﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Messenger.Model;
using Messenger.DataLayer;
using Messenger.DataLayer.SQL;
using System.Web;

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
            Profile profile =  profilesRepository.GetProfile(id);
            if(profile == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }
            return profile;
        }

        [HttpPost]
        [Route("api/profile")]
        public Profile Create([FromBody] Profile profile)
        {
            Profile profileAfter = profilesRepository.CreateProfile(profile);
            if (profile == null)
            {
                throw new HttpException(409, "Этот логин занят.");
            }
            return profileAfter;
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
            List<Chat> chats = profilesRepository.GetProfileChats(id).ToList();
            if (chats == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }
            return chats;
        }
/*
        [HttpGet]
        [Route("api/profile/#{name}#{surname}")]
        public IEnumerable<Profile> GetChats(string name, string surname)
        {
            if (!name.Equals(null) || !surname.Equals(null))
                return profilesRepository.GetProfiles(name, surname);
            else
                return null;
        }
        
        [HttpGet]
        [Route("api/profile/#login#{login}")]
        public Profile GetProfile(string login)
        {
            return profilesRepository.GetProfile(login);
        }
        */
    }
}
