using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Messenger.Model;

namespace Messenger.DataLayer.SQL.Tests
{
    [TestClass]
    public class ProfilesRepositoryTests
    {
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";

        private readonly List<Guid> tempUsers = new List<Guid>();
        private readonly List<Guid> chats = new List<Guid>();

        [TestMethod]
        public void ShouldCreateUser()
        {
            //arrange
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "Admin",
                Avatar = Encoding.UTF8.GetBytes("Avatar"),
                Password = "12345",
                Name = "admin",
                Surname = "admin"
            };

            //act
            var repository = new ProfilesRepository(ConnectionString);
            var result = repository.CreateProfile(profile);

            tempUsers.Add(result.Id);

            //asserts
            Assert.AreEqual(profile.Login, result.Login);
            Assert.AreEqual(profile.Avatar, result.Avatar);
            Assert.AreEqual(profile.Password, result.Password);
            Assert.AreEqual(profile.Name, result.Name);
            Assert.AreEqual(profile.Surname, result.Surname);
        }

        [TestMethod]
        public void ShouldStartChatWithUser()
        {
            //arrange
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "Admin",
                Avatar = Encoding.UTF8.GetBytes("Avatar"),
                Password = "12345",
                Name = "admin",
                Surname = "admin"
            };

            const string chatName = "UserChat";

            //act
            var profileRepository = new ProfilesRepository(ConnectionString);
            var result = profileRepository.CreateProfile(profile);

            tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(ConnectionString, profileRepository);

            var chatBefore = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName,
                ChatMembers = new List<Profile>((new Profile[] { profile })),
            };

            var chat = chatRepository.CreateChat(chatBefore);
            chats.Add(chat.ChatId);
            var userChats = profileRepository.GetProfileChats(profile.Id);
            //asserts
            Assert.AreEqual(chatName, chat.ChatName);
            Assert.AreEqual(profile.Login, chat.ChatMembers.Single().Login);
            Assert.AreEqual(chat.ChatId, userChats.Single().ChatId);
            Assert.AreEqual(chat.ChatName, userChats.Single().ChatName);
        }

        [TestMethod]
        public void ShouldGetProfileById()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "Admin",
                Avatar = Encoding.UTF8.GetBytes("Avatar"),
                Password = "12345",
                Name = "admin",
                Surname = "admin"
            };

            var repository = new ProfilesRepository(ConnectionString);
            repository.CreateProfile(profile);
            tempUsers.Add(profile.Id);

            var result = repository.GetProfile(profile.Id);

            Assert.AreEqual(profile.Login, result.Login);
            Assert.AreEqual(profile.Password, result.Password);
            Assert.AreEqual(profile.Name, result.Name);
            Assert.AreEqual(profile.Surname, result.Surname);
        }

        [TestMethod]
        public void ShouldDeleteProfile()
        {
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "Admin",
                Avatar = Encoding.UTF8.GetBytes("Avatar"),
                Password = "12345",
                Name = "admin",
                Surname = "admin"
            };

            var repository = new ProfilesRepository(ConnectionString);
            repository.CreateProfile(profile);

            Profile result = null;

            repository.DeleteProfile(profile.Id);
            try
            {
                result = repository.GetProfile(profile.Id);
            }
            catch(System.ArgumentException)
            {
                return;
            }
        }

        [TestCleanup]
        public void Clean()
        {
            foreach (var login in tempUsers)
            {
                var user = new ProfilesRepository(ConnectionString);
                foreach (var chat in chats)
                    new ChatsRepository(ConnectionString, user).DeleteChat(chat);
                user.DeleteProfile(login);
            }
        }
    }
}
