using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IChatsRepository
    {
        Chat CreateChat(Chat chat);
        Chat GetChat(Guid chatId);
        void DeleteChat(Guid chatId);
        IEnumerable<Profile> GetChatMembers(Guid chatId);
        void AddChatMember(Guid userId, Guid chatId);
        void DeleteChatMember(Guid userId, Guid chatId);
        IEnumerable<Chat> FindChats(String[] names, Guid profileId);
    }
}
