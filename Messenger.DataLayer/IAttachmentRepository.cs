using System;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IAttachmentRepository
    {
        Attachment LoadAttachment(Attachment file);
        Attachment GetAttachment(Guid id);
        void DeleteAttachment(Guid id);
    }
}
