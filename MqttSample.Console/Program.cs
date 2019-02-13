using M2Mqtt;
using M2Mqtt.Messages;
using System;
using System.Security.Cryptography.X509Certificates;

namespace MqttSample.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello World! TLS and mqtt. Fingers crossed");
            try
            {
                X509Certificate clientCert =
                    new X509Certificate2($"c:/jstuff/tls/karla/selfsigned/consoleclient.pfx", "xamarin");

                X509Certificate caCert =
                    //X509Certificate.CreateFromCertFile("c:/jstuff/tls/karla/selfsigned/karla.redacted.org.crt");
                    X509Certificate.CreateFromCertFile($"c:/jstuff/tls/karla/selfsigned/karla.redacted.org.crt");


                var _client = new MqttClient("redacted.org", 8883, true, caCert, clientCert, MqttSslProtocols.TLSv1_2);

                _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived;
                string clientId = Guid.NewGuid().ToString();
                _client.Connect(clientId);
                _client.Subscribe(new String[] { "xamtest" }, new byte[] { M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                System.Console.ReadKey();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private static void _client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = System.Text.Encoding.Default.GetString(e.Message);
            System.Console.WriteLine($"Message received in MqttDataService _client_MqttMsgPublishReceived {message}");
        }
    }
}
