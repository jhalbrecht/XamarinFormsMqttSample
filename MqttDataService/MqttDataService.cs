using System;
using System.Diagnostics;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xamarin.Essentials;
using MqttDataService.Models;
using Prism.Events;
using Messaging.Models;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Net.Sockets;

namespace MqttDataService
{
    public class MqttDataService : IMqttDataService
    {
        private MqttClient _client;
        private IXpdSettings _xpdSetting;
        private IEventAggregator _eventAggregator;

        X509Certificate dummyX509CertificateA, dummyX509CertificateB;

        public MqttDataService(IXpdSettings xpdSetting, IEventAggregator eventAggregator)
        {
            _xpdSetting = xpdSetting;
            _eventAggregator = eventAggregator;
        }

        public async Task Initialize()
        {
            try
            {
                if (_xpdSetting.UseTls)
                {
                    //MqttClient client = new MqttClient("ppatierno-PC",
                    //    MqttClient.MQTT_BROKER_DEFAULT_SSL_PORT,
                    //    true,
                    //    new X509Certificate(Resources.m2mqtt_ca));

                    // line 310 MqttClient.cs
                    //public MqttClient(string brokerHostName, int brokerPort, bool secure, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol)

                    //var foo = System.Net.Dns.GetHostAddresses(_xpdSetting.MqttBrokerAddress).ToString();

                    _client = new MqttClient(
                        // Dns.GetHostAddresses(_xpdSetting.MqttBrokerAddress).ToString(),
                        GetHostName(_xpdSetting.MqttBrokerAddress),
                        Int32.Parse(_xpdSetting.MqttBrokerTlsPort),
                        _xpdSetting.UseTls,
                                                //X509Certificate caCert,         // TODO: learn
                                                //X509Certificate clientCert,     // TODO: learn
                                                //MqttSslProtocols.TLSv1_2);      // TODO: learn

                         dummyX509CertificateA,                //caCert,         // TODO: learn X509Certificate and how to manipulate
                         dummyX509CertificateB,                //clientCert,     // TODO: learn X509Certificate
                        MqttSslProtocols.TLSv1_2);      // TODO: learn

                    //_client = new MqttClient(_xpdSetting.MqttBrokerAddress);
                }
                else
                {
                    _client = new MqttClient(_xpdSetting.MqttBrokerAddress);
                }


                _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived;
                string clientId = Guid.NewGuid().ToString();
                _client.Connect(clientId);
                _client.Subscribe(new String[] { _xpdSetting.MqttBrokerTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void _client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = System.Text.Encoding.Default.GetString(e.Message);
            Debug.WriteLine($"Message received in MqttDataService _client_MqttMsgPublishReceived {message}");
            _eventAggregator.GetEvent<MessageSentEvent>().Publish(message);
        }

        // https://stackoverflow.com/questions/11123639/how-to-resolve-hostname-from-local-ip-in-c-net
        public string GetHostName(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                //unknown host or
                //not every IP has a name
                //log exception (manage it)

                return ipAddress;
            }
            return null;
        }
    }
}
