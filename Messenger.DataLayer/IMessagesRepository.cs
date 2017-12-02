using System;
using System.Collections.Generic;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IMessagesRepository
    {
        Message SendMessage(Message message);
        Message GetMessage(Guid id);
        void DeleteMessage(Guid id);
        IEnumerable<Message> GetMessages(Guid chatId);
        int CountMessages(Guid chatId);
        int CountReadMessages(Guid chatId, Guid personId);
        IEnumerable<Message> FindMessages(String[] names, Guid profileId);
        void CheckUndestroyedMessages(Guid id);
        void UpdateMessageRead(Guid id);
        void Destroy(Message message);
    }
}
