using Prism.Events;

namespace Messaging.Models
{
    public class MqttMessageTransport : PubSubEvent<MqttMessageTransport>
    {
        // private byte[] payload;
        //private string payload;
        public MqttMessageTransport()
        {
        }
        // public MqttMessageTransport(string topic, byte[] payload)
        public MqttMessageTransport(string topic, string payload)

        {
            Topic = topic;
            Message = payload;
            //this.payload = payload;
        }

        public string Topic { get; set; }
        public string Message { get; set; }
    }
}