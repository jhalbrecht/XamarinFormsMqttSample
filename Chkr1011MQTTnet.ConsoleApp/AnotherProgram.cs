using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using MQTTnet.Serializer;
using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Chkr1011MQTTnet.ConsoleApp.AnotherProgram
{
    class Program
    {
        private static bool remoteValidation(X509Certificate certificate, 
            X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors, MqttClientTcpOptions options)
        { return true; }
        //{ //check certificate and return true or false return true; }
        static void Main(string[] args)
        {
            testi();
            Console.ReadLine();
        }
        private static async void testi()
        {

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            //var mqttClient = new MqttClientFactory().CreateMqttClient();
            var _mqttOptions = new MqttClientTcpOptions();
            _mqttOptions.CleanSession = true;
            _mqttOptions.ClientId = "TEST";
            _mqttOptions.DefaultCommunicationTimeout = TimeSpan.FromSeconds(20);
            _mqttOptions.KeepAlivePeriod = TimeSpan.FromSeconds(31);
            _mqttOptions.Server = "127.0.0.1";
            _mqttOptions.UserName = "MQTTUserName";
            _mqttOptions.Port = 8883;
            _mqttOptions.TlsOptions = new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = true,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true
            };
            MqttTcpChannel.CustomCertificateValidationCallback = remoteValidation;
            await mqttClient.ConnectAsync(_mqttOptions);
            mqttClient.Disconnected += async (s, e) =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(_mqttOptions);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            };
        }
    }
}
