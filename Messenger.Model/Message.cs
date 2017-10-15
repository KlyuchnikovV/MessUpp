﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Model
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public Guid ProfileId { get; set; }
        public Guid ChatId { get; set; }
        public string MessageText { get; set; }
        public DateTime Date { get; set; }
        public int TimeToDestroy { get; set; } // in seconds
        public Guid Attachment { get; set; }
    }
}
