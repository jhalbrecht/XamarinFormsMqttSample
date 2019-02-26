// using Java.Security;
//using M2Mqtt;
//using M2Mqtt.Messages;
//using uPLibrary.Networking.M2Mqtt;
//using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net.Mqtt;

namespace MqttChatApp.XamarinMqttDataServices.Services
{
    internal class MqttCredentials : MqttClientCredentials
    {
        public MqttCredentials(string clientId) : base(clientId)
        {
        }
    }
}