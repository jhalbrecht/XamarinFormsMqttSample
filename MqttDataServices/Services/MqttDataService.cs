// using Java.Security;
using M2Mqtt;
using M2Mqtt.Messages;
//using uPLibrary.Networking.M2Mqtt;
//using uPLibrary.Networking.M2Mqtt.Messages;
using Messaging.Models;
using MqttChattApp.Utility.Services;
//using MqttSample.Utility.Services;
using Prism.Events;
using Prism.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

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
            // ToDo: saw it on so ... var mqttClient = new MqttFactory().CreateMqttClient();

            _xpdSetting = xpdSetting;
            _eventAggregator = eventAggregator;
            _pageDialogService = pageDialogService;

            Debug.WriteLine($"\n\n in MqttDataService constructor \n\n");

        }

        public async Task Initialize()
        {
            Debug.WriteLine($"\n\n in MqttDataService Initialize() \n\n");

            if (Device.RuntimePlatform == Device.UWP)
                Debug.WriteLine($"\nUWP\n");

            if (Device.RuntimePlatform == Device.Android)
                Debug.WriteLine($"\nAndroid\n");

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

                    byte[] certificate = Convert.FromBase64String(theBase64EncodedPfx);
                    X509Certificate2 clientCert = new X509Certificate2(certificate, "xamarin");

                    //            [Android.Runtime.Register("java/security/KeyStore", DoNotGenerateAcw = true)]
                    //public class KeyStore : Object
                    //{
                    //}

                    //var certStore = KeyStore.GetInstance("AndroidCAStore");
                    //[Android.Runtime.Register("java/security/KeyStore", DoNotGenerateAcw = true)]

                    // ToDo: Study this several times; https://stackoverflow.com/questions/3027273/how-to-store-and-load-keys-using-java-security-keystore-class




                    //var certStore = KeyStore.GetInstance("AndroidCAStore");
                    //certStore.Load(null);

                    /*
                     * https://stackoverflow.com/questions/45090618/xamarin-visual-studio-createcertfromfile-path-no-working
                    AssetManager assets = this.Assets;
                    using (Stream input = assets.Open("ca-bundle.crt"))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int read;
                            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            data = ms.ToArray();
                        }
                    }*/

                    //Account account = new Account(...);
                    //AccountStore store = AccountStore.Create();
                    //store.Save(account, appName);

                    // ToDo: research https://stackoverflow.com/questions/28276182/invalid-certificate-received-from-server-xamarin-android/28550770
                    //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });



                    //
                    // We have the certificates, connect to the broker
                    //

                    LogCertificatInformation(clientCert);
                    // LogX509ChainInformation(clientCert);
                    // DoX509ExtensionClass();

                    _client = new MqttClient(
                        GetHostName(_xpdSetting.MqttBrokerAddress),
                        Int32.Parse(_xpdSetting.MqttBrokerTlsPort),
                        _xpdSetting.UseTls,
                        caCert,
                        clientCert,
                        MqttSslProtocols.TLSv1_2
                        //MyRemoteCertificateValidationCallback
                        );

                }
                else
                {
                    //_client = new MqttClient(_xpdSetting.MqttBrokerAddress);
                    _client = new MqttClient(_xpdSetting.MqttBrokerAddress);

                }

                // Android permissions?
                // https://github.com/eclipse/paho.mqtt.android/issues/98

                // .pfx on phone bac7d4ea62c691f452ba9535bf786e11ddfb6824

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

        private static void LogX509ChainInformation(X509Certificate2 x509)
        {
            //Output chain information of the selected certificate.
            X509Chain ch = new X509Chain();
            ch.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            ch.Build(x509);
            Debug.WriteLine("\nChain Information");
            Debug.WriteLine("Chain revocation flag: {0}", ch.ChainPolicy.RevocationFlag);
            Debug.WriteLine("Chain revocation mode: {0}", ch.ChainPolicy.RevocationMode);
            Debug.WriteLine("Chain verification flag: {0}", ch.ChainPolicy.VerificationFlags);
            Debug.WriteLine("Chain verification time: {0}", ch.ChainPolicy.VerificationTime);
            Debug.WriteLine("Chain status length: {0}", ch.ChainStatus.Length);
            Debug.WriteLine("Chain application policy count: {0}", ch.ChainPolicy.ApplicationPolicy.Count);
            Debug.WriteLine("Chain certificate policy count: {0} {1}\n", ch.ChainPolicy.CertificatePolicy.Count, Environment.NewLine);
        }

        private static void DoX509ExtensionClass()
        {
            try
            {
                X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                for (int i = 0; i < collection.Count; i++)
                {
                    foreach (X509Extension extension in collection[i].Extensions)
                    {
                        Console.WriteLine(extension.Oid.FriendlyName + "(" + extension.Oid.Value + ")");


                        if (extension.Oid.FriendlyName == "Key Usage")
                        {
                            X509KeyUsageExtension ext = (X509KeyUsageExtension)extension;
                            Console.WriteLine(ext.KeyUsages);
                        }

                        if (extension.Oid.FriendlyName == "Basic Constraints")
                        {
                            X509BasicConstraintsExtension ext = (X509BasicConstraintsExtension)extension;
                            Console.WriteLine(ext.CertificateAuthority);
                            Console.WriteLine(ext.HasPathLengthConstraint);
                            Console.WriteLine(ext.PathLengthConstraint);
                        }

                        if (extension.Oid.FriendlyName == "Subject Key Identifier")
                        {
                            X509SubjectKeyIdentifierExtension ext = (X509SubjectKeyIdentifierExtension)extension;
                            Console.WriteLine(ext.SubjectKeyIdentifier);
                        }

                        if (extension.Oid.FriendlyName == "Enhanced Key Usage")
                        {
                            X509EnhancedKeyUsageExtension ext = (X509EnhancedKeyUsageExtension)extension;
                            OidCollection oids = ext.EnhancedKeyUsages;
                            foreach (Oid oid in oids)
                            {
                                Console.WriteLine(oid.FriendlyName + "(" + oid.Value + ")");
                            }
                        }
                    }
                }
                store.Close();
            }
            catch (CryptographicException)
            {
                Console.WriteLine("Information could not be written out for this certificate.");
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
                Debug.WriteLine($"ex: {ex}");
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
            // this foreach was handy while learning and trying to get more detail on policy errors.

            foreach (X509ChainStatus item in chain.ChainStatus)
            {
                Debug.WriteLine($"\nX509ChainStatus item: {item.StatusInformation}\n");
            }

            // return true;            // ToDo: ignore the error while testing. Do I need to install my root ca in the UWP app?

            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        //private void Thing()
        //{
        //    private Key generateAndStoreKey(String keyName, KEY_TYPE keyType, KeyPair keyPair) throws IOException {
        //        Key key = null;
        //        switch (keyType)
        //        {
        //            case PUBLIC:
        //                key = keyPair.getPublic();
        //                break;
        //            case PRIVATE:
        //                key = keyPair.getPrivate();
        //                break;
        //        }
        //        byte[] keyBytes = key.getEncoded();
        //        File keyFile = new File("D:\\MySecurityKeys\" + keyName);


        //        FileOutputStream fos = new FileOutputStream(keyFile);
        //        fos.write(keyBytes);
        //        fos.flush();
        //        fos.close();

        //        return key;
        //    }
        //}
    }
}
