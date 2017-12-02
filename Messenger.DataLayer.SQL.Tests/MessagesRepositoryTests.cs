using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Messenger.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Messenger.DataLayer.SQL.Tests
{
    [TestClass]
    public class MessagesRepositoryTests
    {
        private readonly List<Guid> _chats = new List<Guid>();
        private readonly List<Guid> _messages = new List<Guid>();
        private readonly List<Guid> _tempUsers = new List<Guid>();

        [TestMethod]
        public void ShouldSendMessage()
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

            const string chatName = "SendChat";

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

            var messageRepository = new MessagesRepository(Constants.Constants.ConnectionString);

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
            _messages.Add(message.MessageId);

            var resultMessage = messageRepository.CreateMessage(message);

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
                Login = "odmin",
                Avatar = Guid.NewGuid(),
                Password = "12345",
                Name = "odmin",
                Surname = "odmin"
            };

            const string chatName = "SendChat";

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

            var messageRepository = new MessagesRepository(Constants.Constants.ConnectionString);

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
            _messages.Add(message.MessageId);

            messageRepository.CreateMessage(message);

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
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        public void ShouldDeleteMessage()
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

            const string chatName = "SendChat";

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

            var messageRepository = new MessagesRepository(Constants.Constants.ConnectionString);

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
            _messages.Add(message.MessageId);

            messageRepository.CreateMessage(message);

            messageRepository.DeleteMessage(message.MessageId);

            try
            {
                messageRepository.GetMessage(message.MessageId);
            }
            catch (Exception)
            {
                return;
            }
        }

        [TestCleanup]
        public void Clean()
        {
            foreach (var login in _tempUsers)
            {
                var user = new ProfilesRepository(Constants.Constants.ConnectionString);
                foreach (var chat in _chats)
                {
                    var chatRepo = new ChatsRepository(Constants.Constants.ConnectionString, user);
                    foreach (var message in _messages)
                        new MessagesRepository(Constants.Constants.ConnectionString).DeleteMessage(message);
                    chatRepo.DeleteChat(chat);
                }
                user.DeleteProfile(login);
            }
        }
    }
}