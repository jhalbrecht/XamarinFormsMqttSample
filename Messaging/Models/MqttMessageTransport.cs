using Prism.Events;

namespace Messaging.Models
{
    public class MqttMessageTransport : PubSubEvent<MqttMessageTransport>
    {
        public string Topic { get; set; }
        public string Message { get; set; }
    }
}