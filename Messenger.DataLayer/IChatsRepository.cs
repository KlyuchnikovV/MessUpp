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
        Chat CreateChat(IEnumerable<Guid> members, string name);
        IEnumerable<Chat> GetProfileChats(Guid id);
        Chat GetChat(Guid chatId);
        void DeleteChat(Guid chatId);

        IEnumerable<Profile> GetChatMembers(Guid chatId);
    }
}
