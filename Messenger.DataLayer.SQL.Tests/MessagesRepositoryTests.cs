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
    public class MessagesRepositoryTests
    {
        private const string ConnectionString = @"Data Source = GORDON-PC\SQLEXPRESS;
                                                  Initial Catalog=MessengerDB; 
                                                  Integrated Security=TRUE; ";

        private readonly List<Guid> tempUsers = new List<Guid>();
        private readonly List<Guid> chats = new List<Guid>();
        private readonly List<Guid> messages = new List<Guid>();

        [TestMethod]
        public void ShouldSendMessage()
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

            const string chatName = "SendChat";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var result = usersRepository.CreateProfile(profile);

            tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat = chatRepository.CreateChat(new[] { profile.Id }, chatName);
            chats.Add(chat.ChatId);

            var messageRepository = new MessagesRepository(ConnectionString);

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                ProfileId = profile.Id,
                ChatId = chat.ChatId,
                MessageText = "Hello, world!",
                Date = DateTime.Now,
                TimeToDestroy = 0,
                Attachment = Guid.Empty
            };
            messages.Add(message.MessageId);

            var resultMessage = messageRepository.SendMessage(message);

            Assert.AreEqual(message.MessageId, resultMessage.MessageId);
            Assert.AreEqual(message.ProfileId, resultMessage.ProfileId);
            Assert.AreEqual(message.ChatId, resultMessage.ChatId);
            Assert.AreEqual(message.MessageText, resultMessage.MessageText);
            Assert.AreEqual(message.Date, resultMessage.Date);
            Assert.AreEqual(message.TimeToDestroy, resultMessage.TimeToDestroy);
            Assert.AreEqual(message.Attachment, resultMessage.Attachment);

        }

        [TestMethod]
        public void ShouldGetMessage()
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

            const string chatName = "SendChat";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var result = usersRepository.CreateProfile(profile);

            tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat = chatRepository.CreateChat(new[] { profile.Id }, chatName);
            chats.Add(chat.ChatId);

            var messageRepository = new MessagesRepository(ConnectionString);

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                ProfileId = profile.Id,
                ChatId = chat.ChatId,
                MessageText = "Hello, world!",
                Date = DateTime.Now,
                TimeToDestroy = 0,
                Attachment = Guid.Empty
            };
            messages.Add(message.MessageId);

            messageRepository.SendMessage(message);

            var resultMessage = messageRepository.GetMessage(message.MessageId);

            Assert.AreEqual(message.MessageId, resultMessage.MessageId);
            Assert.AreEqual(message.ProfileId, resultMessage.ProfileId);
            Assert.AreEqual(message.ChatId, resultMessage.ChatId);
            Assert.AreEqual(message.MessageText, resultMessage.MessageText);
            Assert.AreEqual(message.Date.Date, resultMessage.Date.Date);
            Assert.AreEqual(message.TimeToDestroy, resultMessage.TimeToDestroy);
            Assert.AreEqual(message.Attachment, resultMessage.Attachment);
        }

        [TestMethod]
        public void ShouldDeleteMessage()
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

            const string chatName = "SendChat";

            var usersRepository = new ProfilesRepository(ConnectionString);
            var result = usersRepository.CreateProfile(profile);

            tempUsers.Add(result.Id);

            var chatRepository = new ChatsRepository(ConnectionString, usersRepository);

            var chat = chatRepository.CreateChat(new[] { profile.Id }, chatName);
            chats.Add(chat.ChatId);

            var messageRepository = new MessagesRepository(ConnectionString);

            var message = new Message
            {
                MessageId = Guid.NewGuid(),
                ProfileId = profile.Id,
                ChatId = chat.ChatId,
                MessageText = "Hello, world!",
                Date = DateTime.Now,
                TimeToDestroy = 0,
                Attachment = Guid.Empty
            };
            messages.Add(message.MessageId);

            var resultMessage = messageRepository.SendMessage(message);

            messageRepository.DeleteMessage(message.MessageId);

            try
            {
                resultMessage = messageRepository.GetMessage(message.MessageId);
            }
            catch (System.ArgumentException)
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
                {
                    var chatRepo = new ChatsRepository(ConnectionString, user);
                    foreach (var message in messages)
                        new MessagesRepository(ConnectionString).DeleteMessage(message);
                    chatRepo.DeleteChat(chat);
                }
                user.DeleteProfile(login);
            }
        }
    }
}
