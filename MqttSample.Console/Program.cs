﻿using M2Mqtt;
using M2Mqtt.Messages;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MqttSample.ConsoleApp
{
    // I want to check this out for a possible let's encrypt solution
    // MqttClient client = new MqttClient("ppatierno-PC",
    //    MqttClient.MQTT_BROKER_DEFAULT_SSL_PORT,
    //    true,
    //    new X509Certificate(Resources.m2mqtt_ca));

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("mqtt and TLS with self signed certificate.\nDid you install your cert in Trusted root certification authorities on this machine?\n");
            try
            {
                X509Certificate clientCert =
                    new X509Certificate2($"c:/jstuff/tls/debbie/selfsigned/xamarinclient.pfx", "xamarin");

                X509Certificate caCert =
                        X509Certificate.CreateFromCertFile($"c:/jstuff/tls/debbie/selfsigned/ca.crt");

                MqttClient _client = new MqttClient("redacted.org", 8883, true, caCert, clientCert, MqttSslProtocols.TLSv1_2, MyRemoteCertificateValidationCallback);

                _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived;
                //string clientId = Guid.NewGuid().ToString();
                string clientId = "xamarin";
                _client.Connect(clientId);
                _client.Subscribe(new String[] { "xamtest" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                // Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // this was handy while learning and trying to get more detail on policy errors.
            foreach (X509ChainStatus item in chain.ChainStatus)
            {
                Console.WriteLine($"{item.StatusInformation}");
            }

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            // Console.ReadKey();
            return false;
        }

        private static void _client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = System.Text.Encoding.Default.GetString(e.Message);
            Console.WriteLine($"Message received in MqttDataService _client_MqttMsgPublishReceived {message}");
        }
    }
}
