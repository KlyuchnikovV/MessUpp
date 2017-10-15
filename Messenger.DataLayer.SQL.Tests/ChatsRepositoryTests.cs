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
    public class ChatsRepositoryTests
    {
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";

        private readonly List<Guid> tempUsers = new List<Guid>();
        private readonly List<Guid> chats = new List<Guid>();

        [TestMethod]
        public void ShouldCreateChat()
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

            const string chatName = "CreateChat";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var result = usersRepository.CreateProfile(profile);

            tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat = chatRepository.CreateChat(new[] { profile.Id }, chatName);
            chats.Add(chat.ChatId);
            var userChats = chatRepository.GetProfileChats(profile.Id);

            Assert.AreEqual(chatName, chat.ChatName);
            Assert.AreEqual(profile.Login, chat.ChatMembers.Single().Login);
            Assert.AreEqual(chat.ChatId, userChats.Single().ChatId);
            Assert.AreEqual(chat.ChatName, userChats.Single().ChatName);
        }

        [TestMethod]
        public void ShouldGetChat()
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

            const string chatName = "GetChat";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var result = usersRepository.CreateProfile(profile);

            tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat = chatRepository.CreateChat(new[] { profile.Id }, chatName);
            chats.Add(chat.ChatId);
            var resultChatById = chatRepository.GetChat(chat.ChatId);

            Assert.AreEqual(chat.ChatName, resultChatById.ChatName);
        }

        [TestMethod]
        public void ShouldDeleteChat()
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

            const string chatName = "DeleteChat";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var resProfile = usersRepository.CreateProfile(profile);

            tempUsers.Add(resProfile.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat = chatRepository.CreateChat(new[] { profile.Id }, chatName);
            chats.Add(chat.ChatId);
            Chat result = null;

            chatRepository.DeleteChat(chat.ChatId);
            try
            {
                result = chatRepository.GetChat(chat.ChatId);
            }
            catch (System.ArgumentException)
            {
                return;
            }
        }

        [TestMethod]
        public void ShouldGetProfileChats()   
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

            const string chatName1 = "ProfileChat#1";
            const string chatName2 = "ProfileChat#2";
            const string chatName3 = "ProfileChat#3";
            const string chatName4 = "ProfileChat#4";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var resProfile = usersRepository.CreateProfile(profile);

            tempUsers.Add(resProfile.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat1 = chatRepository.CreateChat(new[] { profile.Id }, chatName1);
            var chat2 = chatRepository.CreateChat(new[] { profile.Id }, chatName2);
            var chat3 = chatRepository.CreateChat(new[] { profile.Id }, chatName3);
            var chat4 = chatRepository.CreateChat(new[] { profile.Id }, chatName4);
            chats.Add(chat1.ChatId);
            chats.Add(chat2.ChatId);
            chats.Add(chat3.ChatId);
            chats.Add(chat4.ChatId);

            List<Chat> profileChats = chatRepository.GetProfileChats(profile.Id).ToList();

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
            var adminProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "Admin",
                Avatar = Encoding.UTF8.GetBytes("Avatar"),
                Password = "12345",
                Name = "admin",
                Surname = "admin"
            };

            var userProfile = new Profile
            {
                Id = Guid.NewGuid(),
                Login = "User",
                Avatar = Encoding.UTF8.GetBytes("Avatar1"),
                Password = "54321",
                Name = "user",
                Surname = "user"
            };

            const string chatName = "AddChat";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var resAdminProfile = usersRepository.CreateProfile(adminProfile);
            var resUserProfile = usersRepository.CreateProfile(userProfile);

            tempUsers.Add(resAdminProfile.Id);
            tempUsers.Add(resUserProfile.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat = chatRepository.CreateChat(new[] { resAdminProfile.Id },  chatName);
            chats.Add(chat.ChatId);
            chatRepository.AddChatMember(resUserProfile.Id, chat.ChatId);

            var userChats = chatRepository.GetProfileChats(resUserProfile.Id).ToList();

            Assert.AreEqual(chat.ChatId, userChats[0].ChatId);
            Assert.AreEqual(chat.ChatName, userChats[0].ChatName);


            chatRepository.DeleteChatMember(resUserProfile.Id, chat.ChatId);

            userChats = chatRepository.GetProfileChats(resUserProfile.Id).ToList();
            try
            {
                Assert.AreEqual(chat.ChatId, userChats[0].ChatId);
                Assert.AreEqual(chat.ChatName, userChats[0].ChatName);
            }
            catch(ArgumentOutOfRangeException)
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
