namespace MqttSample.Utility.Services
{
    public interface IXpdSettings
    {
        string MqttBrokerAddress { get; set; }
        string MqttBrokerPort { get; set; }
        string MqttBrokerTlsPort { get; set; }
        string MqttBrokerUserName { get; set; }
        string MqttBrokerUserPassword { get; set; }
        string MqttBrokerTopic { get; set; }
        bool UseTls { get; set; }
    }
}

