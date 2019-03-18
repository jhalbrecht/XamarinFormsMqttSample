//using M2Mqtt;
//using M2Mqtt.Messages;
using MqttChattApp.Utility.Services;
using MqttDataServices.Models;
using Prism.Events;
using Prism.Services;
using System;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xamarin.Forms;

// ToDo: Worth further investagation http://paulstovell.com/blog/x509certificate2
// ToDo: interesting this is available. Further research.
// https://docs.microsoft.com/en-us/dotnet/api/system.net.servicepointmanager?view=netframework-4.7.2
// var a = System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

// ToDo: I want to check this out sometime....
// https://github.com/chkr1011/MQTTnet

// Create a new MQTT client.
//var factory = new MqttFactory();
//var mqttClient = factory.CreateMqttClient();
//var mqttClient = new MqttFactory().CreateMqttClient();

/*                          -->  My hero  <-- 
*                          
* https://stackoverflow.com/questions/42803493/unable-to-instantiate-x509certificate2-from-byte-array
*/

namespace MqttDataServices.Services
{
    public class MqttDataService : IMqttDataService
    {
        private MqttClient _client;
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
            DisplayAlarm("Mqtt chat starting");
            try
            {
                if (_xpdSetting.UseTls)
                {
                    _client = new MqttClient(
                        _xpdSetting.MqttBrokerAddress,
                        Int32.Parse(_xpdSetting.MqttBrokerTlsPort),
                        _xpdSetting.UseTls,
                        MqttSslProtocols.TLSv1_2,
                        new RemoteCertificateValidationCallback(MyRemoteCertificateValidationCallback),
                        new LocalCertificateSelectionCallback(MyLocalCertificateSelectionCallback)
                        );
                }
                else
                {
                    _client = new MqttClient(_xpdSetting.MqttBrokerAddress);
                }

                _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived; // TODO I don't think I like this naming _client_MqttMsgPublishReceived
                _client.ConnectionClosed += _client_ConnectionClosed;

                string clientId = "";
                if (Device.RuntimePlatform == Device.UWP)
                {
                    clientId = "XamarinUWP";
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    clientId = "XamarinAndroid";
                }
                else
                {
                    clientId = "XamariniOS";
                }

                //string clientId = Guid.NewGuid().ToString();

                //_client.Connect(clientId);
                _client.Connect(clientId, null, null, false, 60);
                _client.Subscribe(new String[] { _xpdSetting.MqttBrokerTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void _client_ConnectionClosed(object sender, EventArgs e)
        {
            DisplayAlarm("_client_ConnectionClosed");
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
        public void PublishMqttMessage(string publishmessage)
        {
            if (_client.IsConnected)
            {
                _client.Publish(
                    _xpdSetting.MqttBrokerTopic,
                    System.Text.Encoding.UTF8.GetBytes(publishmessage));
            }
            else
            {
                DisplayAlarm("PublishMqttMessage client is disconnected");
            }
        }
        private void DisplayAlarm(string alarmMessage)
        {
            Debug.WriteLine($"\n{alarmMessage}\n");
            MqttMessageTransport mmt = new MqttMessageTransport();
            string now = string.Format("{0:HH:mm:ss tt}", DateTime.Now);
            mmt.Topic = "ALARM";
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
