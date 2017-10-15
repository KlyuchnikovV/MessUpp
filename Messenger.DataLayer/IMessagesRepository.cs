using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IMessagesRepository
    {
        Message SendMessage(Message message);
        Message GetMessage(Guid id);
        void DeleteMessage(Guid id);
    }
}
