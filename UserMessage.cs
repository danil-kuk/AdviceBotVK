using System;
using System.Collections.Generic;
using System.Text;
using VkNet.Model;

namespace AdviceBotVK
{
    class UserMessage
    {
        public string Text { get; set; }
        public long FromId { get; set; }
        public Message Message { get; }
        public UserMessage(Message message)
        {
            Message = message;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hash = 23;
            return Message.FromId.Value.GetHashCode() * hash;
        }
    }
}
