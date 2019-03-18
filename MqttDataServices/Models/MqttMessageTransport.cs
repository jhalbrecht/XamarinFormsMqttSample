using Prism.Events;

namespace MqttDataServices.Models
{
    public class MqttMessageTransport : PubSubEvent<MqttMessageTransport>
    {
        public string Topic { get; set; }
        public string Message { get; set; }
    }
}