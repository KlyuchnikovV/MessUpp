using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Messenger.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Messenger.DataLayer.SQL.Tests
{
    [TestClass]
    public class ProfilesRepositoryTests
    {
        private readonly List<Guid> _chats = new List<Guid>();
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldCreateUser()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "odmin",
                Avatar = Guid.NewGuid(),
                Password = "12345",
                Name = "odmin",
                Surname = "odmin"
            };
            var repository = new ProfilesRepository(Constants.Constants.ConnectionString);
            var result = repository.CreateProfile(profile);

            _tempUsers.Add(result.Id);
            Assert.AreEqual(profile.Login, result.Login);
            Assert.AreEqual(profile.Avatar, result.Avatar);
            Assert.AreEqual(profile.Password, result.Password);
            Assert.AreEqual(profile.Name, result.Name);
            Assert.AreEqual(profile.Surname, result.Surname);
        }

        [TestMethod]
        public void ShouldStartChatWithUser()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "odmin",
                Avatar = Guid.NewGuid(),
                Password = "12345",
                Name = "odmin",
                Surname = "odmin"
            };
            const string chatName = "UserChat";
            var profileRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            var result = profileRepository.CreateProfile(profile);
            _tempUsers.Add(result.Id);
            var chatRepository = new ChatsRepository(Constants.Constants.ConnectionString, profileRepository);
            var chatBefore = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };
            var chat = chatRepository.CreateChat(chatBefore);
            _chats.Add(chat.ChatId);
            var userChats = profileRepository.GetProfileChats(profile.Id);
            Assert.AreEqual(chatName, chat.ChatName);
            Assert.AreEqual(profile.Id, chat.ChatMembers.Single());
            var chats = userChats as IList<Chat> ?? userChats.ToList();
            Assert.AreEqual(chat.ChatId, chats.Single().ChatId);
            Assert.AreEqual(chat.ChatName, chats.Single().ChatName);
        }

        [TestMethod]
        public void ShouldGetProfileById()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "odmin",
                Avatar = Guid.NewGuid(),
                Password = "12345",
                Name = "odmin",
                Surname = "odmin"
            };

            var repository = new ProfilesRepository(Constants.Constants.ConnectionString);
            repository.CreateProfile(profile);
            _tempUsers.Add(profile.Id);

            var result = repository.GetProfile(profile.Id);

            Assert.AreEqual(profile.Login, result.Login);
            Assert.AreEqual(profile.Name, result.Name);
            Assert.AreEqual(profile.Surname, result.Surname);
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        public void ShouldDeleteProfile()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "odmin",
                Avatar = Guid.NewGuid(),
                Password = "12345",
                Name = "odmin",
                Surname = "odmin"
            };

            var repository = new ProfilesRepository(Constants.Constants.ConnectionString);
            repository.CreateProfile(profile);

            repository.DeleteProfile(profile.Id);
            try
            {
                repository.GetProfile(profile.Id);
            }
            catch (Exception)
            {
            }
        }

        [TestCleanup]
        public void Clean()
        {
            foreach (var login in _tempUsers)
            {
                var user = new ProfilesRepository(Constants.Constants.ConnectionString);
                foreach (var chat in _chats)
                    new ChatsRepository(Constants.Constants.ConnectionString, user).DeleteChat(chat);
                user.DeleteProfile(login);
            }
        }
    }
}