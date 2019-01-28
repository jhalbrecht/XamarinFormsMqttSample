using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging.Models
{
    public class MessageSentEvent : PubSubEvent<string>
    {
    }
}
