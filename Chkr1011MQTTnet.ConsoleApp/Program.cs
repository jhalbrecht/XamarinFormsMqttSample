using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Implementations;
using MQTTnet.Serializer;
using System;
using System.Text;

namespace Chkr1011MQTTnet.ConsoleApp
{
    // https://github.com/chkr1011/MQTTnet/wiki/Client
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello chkr1011/MQTTnet World! See; https://github.com/chkr1011/MQTTnet/wiki/Client");

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var _mqttOptions = new MqttClientTcpOptions();
            _mqttOptions.TlsOptions = new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = true,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true
            };
            var options = new MqttClientOptionsBuilder()
                .WithClientId("Chkr1011MQTTnetConsoleApp")
                .WithTcpServer("iot.eclipse.org")
                .WithProtocolVersion(MqttProtocolVersion.V311)
                //.WithCredentials("bud", "%spencer%")
                //.WithTls()
                //.TlsEndpointOptions.Certificate = new X509Certificate2(File.ReadAllBytes(@"C:\Certs\cert.pem")).RawData; 


                //options.TlsOptions = new MqttClientTlsOptions
                //{
                //    UseTls = true,
                //    AllowUntrustedCertificates = true,
                //    IgnoreCertificateChainErrors = true,
                //    IgnoreCertificateRevocationErrors = true
                //};


                .WithCleanSession()
                .Build();

             mqttClient.ConnectAsync(options);

            mqttClient.Connected += async (s, e) =>
            {
                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("xamtest").Build());
            };

            mqttClient.ApplicationMessageReceived += (s, e) =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            };

            MqttTcpChannel.CustomCertificateValidationCallback = (x509Certificate, x509Chain, sslPolicyErrors, mqttClientTcpOptions) =>
            {
                if (mqttClientTcpOptions.Server == "server_with_revoked_cert")
                {
                    return true;
                }

                return false;
            };

            Console.ReadKey();
        }
    }
}
