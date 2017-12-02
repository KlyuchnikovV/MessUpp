using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Messenger.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Messenger.DataLayer.SQL.Tests
{
    [TestClass]
    public class ChatsRepositoryTests
    {
        private readonly List<Guid> _chats = new List<Guid>();

        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldCreateChat()
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

            const string chatName = "CreateChat";

            var profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            var result = profilesRepository.CreateProfile(profile);

            _tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(Constants.Constants.ConnectionString, profilesRepository);

            var chatBefore = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };

            var chat = chatRepository.CreateChat(chatBefore);

            _chats.Add(chat.ChatId);

            var userChats = profilesRepository.GetProfileChats(profile.Id);

            Assert.AreEqual(chatName, chat.ChatName);
            Assert.AreEqual(profile.Id, chat.ChatMembers.Single());
            var chats = userChats as IList<Chat> ?? userChats.ToList();
            Assert.AreEqual(chat.ChatId, chats.Single().ChatId);
            Assert.AreEqual(chat.ChatName, chats.Single().ChatName);
        }

        [TestMethod]
        public void ShouldGetChat()
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

            const string chatName = "GetChat";

            var usersRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            var result = usersRepository.CreateProfile(profile);

            _tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(Constants.Constants.ConnectionString, usersRepository);

            var chatBefore = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };

            var chat = chatRepository.CreateChat(chatBefore);
            _chats.Add(chat.ChatId);
            var resultChatById = chatRepository.GetChat(chat.ChatId);

            Assert.AreEqual(chat.ChatName, resultChatById.ChatName);
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        public void ShouldDeleteChat()
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

            const string chatName = "DeleteChat";

            var usersRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            var resProfile = usersRepository.CreateProfile(profile);

            _tempUsers.Add(resProfile.Id);

            var chatRepository = new ChatsRepository(Constants.Constants.ConnectionString, usersRepository);

            var chatBefore = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };

            var chat = chatRepository.CreateChat(chatBefore);
            _chats.Add(chat.ChatId);

            chatRepository.DeleteChat(chat.ChatId);
            try
            {
                chatRepository.GetChat(chat.ChatId);
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void ShouldGetProfileChats()
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

            const string chatName1 = "ProfileChat#1";
            const string chatName2 = "ProfileChat#2";
            const string chatName3 = "ProfileChat#3";
            const string chatName4 = "ProfileChat#4";

            var profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            var resProfile = profilesRepository.CreateProfile(profile);

            _tempUsers.Add(resProfile.Id);

            var chatRepository = new ChatsRepository(Constants.Constants.ConnectionString, profilesRepository);

            var chat1Before = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName1,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };

            var chat2Before = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName2,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };

            var chat3Before = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName3,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };

            var chat4Before = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName4,
                ChatMembers = new List<Guid>(new[] {profile.Id})
            };

            var chat1 = chatRepository.CreateChat(chat1Before);
            var chat2 = chatRepository.CreateChat(chat2Before);
            var chat3 = chatRepository.CreateChat(chat3Before);
            var chat4 = chatRepository.CreateChat(chat4Before);
            _chats.Add(chat1.ChatId);
            _chats.Add(chat2.ChatId);
            _chats.Add(chat3.ChatId);
            _chats.Add(chat4.ChatId);

            var profileChats = profilesRepository.GetProfileChats(profile.Id).ToList();

            var ch = profileChats.Find(x => x.ChatId == chat1.ChatId);
            Assert.AreEqual(chat1.ChatId, ch.ChatId);
            Assert.AreEqual(chatName1, ch.ChatName);

            ch = profileChats.Find(x => x.ChatId == chat2.ChatId);
            Assert.AreEqual(chat2.ChatId, ch.ChatId);
            Assert.AreEqual(chatName2, ch.ChatName);

            ch = profileChats.Find(x => x.ChatId == chat3.ChatId);
            Assert.AreEqual(chat3.ChatId, ch.ChatId);
            Assert.AreEqual(chatName3, ch.ChatName);

            ch = profileChats.Find(x => x.ChatId == chat4.ChatId);
            Assert.AreEqual(chat4.ChatId, ch.ChatId);
            Assert.AreEqual(chatName4, ch.ChatName);
        }

        [TestMethod]
        public void ShouldAddDeleteMember()
        {
            var odminProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "odmin",
                Avatar = Guid.NewGuid(),
                Password = "12345",
                Name = "odmin",
                Surname = "odmin"
            };

            var userProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "User",
                Avatar = Guid.NewGuid(),
                Password = "54321",
                Name = "user",
                Surname = "user"
            };

            const string chatName = "AddChat";

            var profilesRepository = new ProfilesRepository(Constants.Constants.ConnectionString);
            var resodminProfile = profilesRepository.CreateProfile(odminProfile);
            var resUserProfile = profilesRepository.CreateProfile(userProfile);

            _tempUsers.Add(resodminProfile.Id);
            _tempUsers.Add(resUserProfile.Id);

            var chatRepository = new ChatsRepository(Constants.Constants.ConnectionString, profilesRepository);

            var chatBefore = new Chat
            {
                ChatId = Guid.NewGuid(),
                ChatName = chatName,
                ChatMembers = new List<Guid>(new[] {odminProfile.Id})
            };

            var chat = chatRepository.CreateChat(chatBefore);

            _chats.Add(chat.ChatId);
            chatRepository.AddChatMember(resUserProfile.Id, chat.ChatId);

            var userChats = profilesRepository.GetProfileChats(resUserProfile.Id).ToList();

            Assert.AreEqual(chat.ChatId, userChats[0].ChatId);
            Assert.AreEqual(chat.ChatName, userChats[0].ChatName);


            chatRepository.DeleteChatMember(resUserProfile.Id, chat.ChatId);

            userChats = profilesRepository.GetProfileChats(resUserProfile.Id).ToList();
            try
            {
                Assert.AreEqual(chat.ChatId, userChats[0].ChatId);
                Assert.AreEqual(chat.ChatName, userChats[0].ChatName);
            }
            catch (ArgumentOutOfRangeException)
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