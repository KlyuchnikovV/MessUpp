using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Model
{
    public class Chat
    {
        public Guid ChatId { get; set; }
        public string ChatName { get; set; }
        public IEnumerable<Profile> ChatMembers { get; set; }
    }
}
