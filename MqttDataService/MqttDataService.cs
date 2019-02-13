using Messaging.Models;
//using MqttDataService.Models;
using MqttSample.Utility.Services;
using Prism.Events;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

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
            // TODO saw it on so ... var mqttClient = new MqttFactory().CreateMqttClient();

            _xpdSetting = xpdSetting;
            _eventAggregator = eventAggregator;
        }

        public async Task Initialize()
        {
            try
            {
                if (_xpdSetting.UseTls)
                {
                    //X509Certificate caCert = X509Certificate.CreateFromCertFile("mosquitto.org.cer");
                    //X509Certificate foocaCert = X509Certificate.CreateFromCertFile("mosquitto.org.cer");
                    // MqttClient(string brokerHostName, int brokerPort, bool secure, X509Certificate caCert)

                    // TODO: I'd like to duplicate this; "mosquitto_pub -h redacted.org -t test -m "`date` hello again" -p 8883 --capath /etc/ssl/certs/"
                    // var fooclient = new MqttClient(_xpdSetting.MqttBrokerAddress, Int32.Parse(_xpdSetting.MqttBrokerTlsPort), true, foocaCert); // bummer no constructor like this.

                    // https://github.com/eclipse/paho.mqtt.m2mqtt/issues/67#issuecomment-361821305

                    try
                    {
                        // public MqttClient(string brokerHostName, int brokerPort, bool secure, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol)

                        // check this out sometime https://gist.github.com/adrenalinehit/ccfeba90264a02fb629f

                        //X509Certificate2 clientCert = new X509Certificate2("poit", "splat");
                        //X509Certificate caCert = X509Certificate.CreateFromCertFile("mosquitto.org.cer");

                        X509Certificate clientCert = new X509Certificate2("ca.crt");
                        X509Certificate caCert = X509Certificate.CreateFromCertFile("dalia.redacted.org.crt");

                        _client = new MqttClient(
                            GetHostName(_xpdSetting.MqttBrokerAddress),
                            Int32.Parse(_xpdSetting.MqttBrokerTlsPort),
                            _xpdSetting.UseTls,
                            caCert,                //caCert,         // TODO: learn X509Certificate and how to manipulate
                            clientCert,                //clientCert,     // TODO: learn X509Certificate
                            MqttSslProtocols.TLSv1_2);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message) ;
                    }
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
            MqttMessageTransport mmt = new MqttMessageTransport();
            mmt.Topic = e.Topic;
            mmt.Message = message;
            Debug.WriteLine($"Message received in MqttDataService _client_MqttMsgPublishReceived {message}");
            _eventAggregator.GetEvent<MqttMessageTransport>().Publish(mmt);
        }

        /// <summary>
        /// based on
        ///  https://stackoverflow.com/questions/11123639/how-to-resolve-hostname-from-local-ip-in-c-net
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>

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
                return ipAddress;
            }
            return null;
        }

        public void PublishMqttMessage(string publishmessage)
        {
            _client.Publish(
                _xpdSetting.MqttBrokerTopic,
                System.Text.Encoding.UTF8.GetBytes(publishmessage));
        }
    }
}