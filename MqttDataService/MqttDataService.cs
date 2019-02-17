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
            // TODO saw it on so ... var mqttClient = new MqttFactory().CreateMqttClient();

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
                    try
                    {
                        bool doesExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ca.crt"));
                        Debug.WriteLine(doesExist ? "ca.crt exist? " + "true" : "false");
                        if (!doesExist)
                        {
                            Debug.WriteLine("Doh! That file doesn't exist");
                            // ToDO: await _pageDialogService.DisplayAlertAsync("my alert", "hello", "OK
                        }
                        X509Certificate caCert =
                            X509Certificate.CreateFromCertFile(
                                Path.Combine(Environment.GetFolderPath(
                                    Environment.SpecialFolder.LocalApplicationData), "ca.crt"));

                        bool doesExistpfx = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "xamarinclient.pfx"));
                        Debug.WriteLine(doesExistpfx ? "xamarinclient.pfx exist? " + "true" : "false");

                        if (!doesExistpfx)
                        {
                            Debug.WriteLine("Doh! That file doesn't exist");
                            // ToDo: await _pageDialogService.DisplayAlertAsync("my alert", "hello", "OK");
                        }

                        string thePathOnDevice = Path.Combine(Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData), "xamarinclient.pfx");

                        var theBase64EncodedPfx = File.ReadAllText(thePathOnDevice);
                        var toip = Convert.FromBase64String(theBase64EncodedPfx);
                        X509Certificate2 pfxcert = new X509Certificate2();

                        //splat.Import(toip, "xamarin", X509KeyStorageFlags.DefaultKeySet);
                        pfxcert.Import(theBase64EncodedPfx); // this complains and needs the password 'xamarin'
                                                             ////X509Certificate clientCert = new X509Certificate2(foo, "xamarin");
                        Debug.WriteLine("fooE");

                        // ToDo: Worth further investagation http://paulstovell.com/blog/x509certificate2

                        X509Certificate clientCert = new X509Certificate2(thePathOnDevice, "xamarin");


                        _client = new MqttClient(
                            GetHostName(_xpdSetting.MqttBrokerAddress),
                            Int32.Parse(_xpdSetting.MqttBrokerTlsPort),
                            _xpdSetting.UseTls,
                            caCert,
                            clientCert,
                            MqttSslProtocols.TLSv1_2,
                            MyRemoteCertificateValidationCallback);

                        // ToDo: I want to check this out sometime....
                        // https://github.com/chkr1011/MQTTnet

                        // Create a new MQTT client.
                        //var factory = new MqttFactory();
                        //var mqttClient = factory.CreateMqttClient();

                        //var mqttClient = new MqttFactory().CreateMqttClient();

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"\n{e.Message}\n");
                        throw;
                    }
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
        private static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // this was handy while learning and trying to get more detail on policy errors.
            foreach (X509ChainStatus item in chain.ChainStatus)
            {
                Debug.WriteLine($"{item.StatusInformation}");
            }

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}