using MqttChattApp.Utility.Services;
using MqttDataServices.Models;
using MQTTnet.Client.Options;
using MQTTnet;
using MQTTnet.Client;
using Prism.Events;
using Prism.Services;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Text;
using System.Threading;

namespace MqttDataServices.Services
{
    public class MqttDataService : IMqttDataService
    {
        private IMqttClient _client;
        private static IMqttClientOptions _options;
        private IXpdSettings _xpdSetting;
        private IEventAggregator _eventAggregator;
        private IPageDialogService _pageDialogService;

        public MqttDataService(IXpdSettings xpdSetting, IEventAggregator eventAggregator, IPageDialogService pageDialogService)
        {
            _xpdSetting = xpdSetting;
            _eventAggregator = eventAggregator;
            _pageDialogService = pageDialogService;
            Debug.WriteLine($"\n\n in MqttDataService constructor \n\n");
        }

        public async Task Initialize()
        {
            Debug.WriteLine($"\n\n in MqttDataService Initialize() \n\n");
            DisplayAlarm("Info", "Mqtt chat starting");

            string clientId = "";
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    clientId = "XamarinChatAppUWP";
                    break;
                case Device.Android:
                    clientId = "XamarinChatAppAndroid";
                    break;
                default:
                    clientId = "XamarinChatAppiOS";
                    break;
            }

            try
            {
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();

                if (_xpdSetting.UseTls)
                {
                    _options = new MqttClientOptionsBuilder()
                        .WithClientId(clientId)
                        .WithTcpServer(_xpdSetting.MqttBrokerAddress, Int16.Parse(_xpdSetting.MqttBrokerTlsPort)) // old M2MqttDotNet used string port. 
                        .WithCredentials(_xpdSetting.MqttBrokerUserName, _xpdSetting.MqttBrokerUserPassword)
                        .WithCleanSession()
                        .WithTls()
                        .Build();
                }
                else
                {
                    _options = new MqttClientOptionsBuilder()
                        .WithClientId(clientId)
                        .WithTcpServer(_xpdSetting.MqttBrokerAddress, Int16.Parse(_xpdSetting.MqttBrokerPort))
                        .WithCredentials(_xpdSetting.MqttBrokerUserName, _xpdSetting.MqttBrokerUserPassword)
                        .WithCleanSession()
                        .Build();
                }

                _client.ConnectAsync(_options).Wait();
                _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_xpdSetting.MqttBrokerTopic).Build()).Wait();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            _client.UseConnectedHandler(e =>
            {
                Debug.WriteLine("Connected successfully with MQTT Brokers.");
                _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_xpdSetting.MqttBrokerTopic).Build()).Wait();
            });

            _client.UseDisconnectedHandler(async e =>
            {
                Debug.WriteLine("\nDisconnect\n");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await _client.ConnectAsync(_options, CancellationToken.None); // Since 3.0.5 with CancellationToken
                    Debug.WriteLine("### Reconnection Success! ###");
                    _client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(_xpdSetting.MqttBrokerTopic).Build()).Wait();
                }
                catch
                {
                    Debug.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            _client.UseApplicationMessageReceivedHandler(e =>
                {
                    MqttMessageTransport mmt = new MqttMessageTransport
                    {
                        Topic = e.ApplicationMessage.Topic,
                        Message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload)
                    };
                    Debug.WriteLine($"Message received in MqttDataService _client_MqttMsgPublishReceived {mmt.Message}");
                    _eventAggregator.GetEvent<MqttMessageTransport>().Publish(mmt);
                });
        }

        private void _client_ConnectionClosed(object sender, EventArgs e)
        {
            DisplayAlarm("Alarm", "_client_ConnectionClosed");
            Debug.WriteLine("connection closed\n");
        }

        public async Task PublishMqttMessageAsync(string publishmessage)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_xpdSetting.MqttBrokerTopic)
                .WithPayload(publishmessage)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await _client.PublishAsync(message, CancellationToken.None); // Since 3.0.5 with CancellationToken
        }
        private void DisplayAlarm(string level, string alarmMessage)
        {
            Debug.WriteLine($"\n{alarmMessage}\n");
            MqttMessageTransport mmt = new MqttMessageTransport();
            // string now = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            mmt.Topic = level;
            mmt.Message = $"{string.Format("{0:HH:mm:ss tt}", DateTime.Now)}; {alarmMessage}";
            _eventAggregator.GetEvent<MqttMessageTransport>().Publish(mmt);
        }

        private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // this foreach was handy while learning and trying to get more detail on policy errors.
            //foreach (X509ChainStatus item in chain.ChainStatus)
            //{
            //    Debug.WriteLine($"\nX509ChainStatus item: {item.StatusInformation}\n");
            //}

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public static X509Certificate MyLocalCertificateSelectionCallback(
            object sender,
            string targetHost,
            X509CertificateCollection localCertificates,
            X509Certificate remoteCertificate,
            string[] acceptableIssuers)
        {
            Debug.WriteLine("Client is selecting a local certificate.");
            if (acceptableIssuers != null &&
                acceptableIssuers.Length > 0 &&
                localCertificates != null &&
                localCertificates.Count > 0)
            {
                // Use the first certificate that is from an acceptable issuer.
                foreach (X509Certificate certificate in localCertificates)
                {
                    string issuer = certificate.Issuer;
                    if (Array.IndexOf(acceptableIssuers, issuer) != -1)
                        return certificate;
                }
            }
            if (localCertificates != null &&
                localCertificates.Count > 0)
                return localCertificates[0];

            return null;
        }
    }
}