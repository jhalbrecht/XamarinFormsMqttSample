using Messaging.Models;
using MqttSample.Utility.Services;
using Prism.Events;
using Prism.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

//using System.IO;
//using Xamarin.Forms;
//using Plugin.FilePicker;
//using Plugin.FilePicker.Abstractions;

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

namespace MqttDataService
{
    public class MqttDataService : IMqttDataService
    {
        private MqttClient _client;
        private IXpdSettings _xpdSetting;
        private IEventAggregator _eventAggregator;
        private IPageDialogService _pageDialogService;

        public MqttDataService(IXpdSettings xpdSetting, IEventAggregator eventAggregator, IPageDialogService pageDialogService)
        {
            // ToDo: saw it on so ... var mqttClient = new MqttFactory().CreateMqttClient();

            _xpdSetting = xpdSetting;
            _eventAggregator = eventAggregator;
            _pageDialogService = pageDialogService;
        }

        public async Task Initialize()
        {
            try
            {
                if (_xpdSetting.UseTls)
                {
                    string filesDirectoryBasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    bool doesExist = File.Exists(Path.Combine(filesDirectoryBasePath, "ca.crt"));
                    Debug.WriteLine(doesExist ? "ca.crt exist? " + "true" : "false");
                    if (!doesExist)
                    {
                        Debug.WriteLine("Doh! That file doesn't exist");
                        // alert user and navigate home.
                        // ToDo: await _pageDialogService.DisplayAlertAsync("my alert", "hello", "OK
                    }

                    X509Certificate caCert = X509Certificate.CreateFromCertFile(Path.Combine(filesDirectoryBasePath, "ca.crt"));

                    bool doesExistpfx = File.Exists(Path.Combine(filesDirectoryBasePath, "xamarinclient.pfx"));
                    Debug.WriteLine(doesExistpfx ? "xamarinclient.pfx exist? " + "true" : "false");
                    if (!doesExistpfx)
                    {
                        Debug.WriteLine("Doh! That file doesn't exist");
                        // ToDo: await _pageDialogService.DisplayAlertAsync("my alert", "hello", "OK"); // hey brian this should work! ;-)
                    }

                    string thePfxPathOnDevice = Path.Combine(filesDirectoryBasePath, "xamarinclient.pfx");
                    string theBase64EncodedPfx = File.ReadAllText(thePfxPathOnDevice);
                    // byte[] pfxByteArrayFromBase64String = Convert.FromBase64String(theBase64EncodedPfx);

                    /*                          -->  My hero  <-- 
                    *                          
                    * https://stackoverflow.com/questions/42803493/unable-to-instantiate-x509certificate2-from-byte-array
                    */

                    byte[] certificate = Convert.FromBase64String(theBase64EncodedPfx);
                    //X509Certificate2 clientCert = new X509Certificate2(certificate, "xamarin");

                    X509Certificate2 clientCert = new X509Certificate2(certificate, "xamarin", X509KeyStorageFlags.DefaultKeySet);

                    //
                    // We have the certificates, connect to the broker
                    //

                    // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2.import?view=netframework-4.7.2#System_Security_Cryptography_X509Certificates_X509Certificate2_Import_System_Byte___System_Security_SecureString_System_Security_Cryptography_X509Certificates_X509KeyStorageFlags_

                    LogCertificatInformation(clientCert);

                    _client = new MqttClient(
                        GetHostName(_xpdSetting.MqttBrokerAddress),
                        Int32.Parse(_xpdSetting.MqttBrokerTlsPort),
                        _xpdSetting.UseTls,
                        caCert,
                        clientCert,
                        MqttSslProtocols.TLSv1_2,
                        MyRemoteCertificateValidationCallback);
                }
                else
                {
                    _client = new MqttClient(_xpdSetting.MqttBrokerAddress);
                }

                _client.MqttMsgPublishReceived += _client_MqttMsgPublishReceived;
                //string clientId = Guid.NewGuid().ToString();
                string clientId = "xamarin";
                _client.Connect(clientId);
                _client.Subscribe(new String[] { _xpdSetting.MqttBrokerTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private static void LogCertificatInformation(X509Certificate2 x509)
        {
            // var x509 = clientCert;
            //Print to console information contained in the certificate.
            Debug.WriteLine("{0}Subject: {1}{0}", Environment.NewLine, x509.Subject);
            Debug.WriteLine("{0}Issuer: {1}{0}", Environment.NewLine, x509.Issuer);
            Debug.WriteLine("{0}Version: {1}{0}", Environment.NewLine, x509.Version);
            Debug.WriteLine("{0}Valid Date: {1}{0}", Environment.NewLine, x509.NotBefore);
            Debug.WriteLine("{0}Expiry Date: {1}{0}", Environment.NewLine, x509.NotAfter);
            Debug.WriteLine("{0}Thumbprint: {1}{0}", Environment.NewLine, x509.Thumbprint);
            Debug.WriteLine("{0}Serial Number: {1}{0}", Environment.NewLine, x509.SerialNumber);
            Debug.WriteLine("{0}Friendly Name: {1}{0}", Environment.NewLine, x509.PublicKey.Oid.FriendlyName);
            Debug.WriteLine("{0}Public Key Format: {1}{0}", Environment.NewLine, x509.PublicKey.EncodedKeyValue.Format(true));
            Debug.WriteLine("{0}Raw Data Length: {1}{0}", Environment.NewLine, x509.RawData.Length);
            Debug.WriteLine("{0}Certificate to string: {1}{0}", Environment.NewLine, x509.ToString(true));
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
        // ToDo: Boy Howdy! This could use some CPR (Copy, Paster and Refactor)
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
        private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // this was handy while learning and trying to get more detail on policy errors.
            // And it was just handy again when trying to implement the FilePicker!

            foreach (X509ChainStatus item in chain.ChainStatus)
            {
                Debug.WriteLine($"\nX509ChainStatus item: {item.StatusInformation}\n");
            }

            return true;            // ToDo: ignore the error while testing. Do I need to install my root ca in the UWP app?


            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}