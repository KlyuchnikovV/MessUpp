using System;

namespace Messenger.Model
{
    public class Attachment
    {
        public Guid AttachId { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
    }
}
