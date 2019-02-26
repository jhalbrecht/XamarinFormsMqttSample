//using M2Mqtt;
//using M2Mqtt.Messages;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

//using uPLibrary.Networking.M2Mqtt;
//using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT.SubscriberTest
{
    public class Program
    {
        /// <summary>
        /// Replace this with your endpoint - it's shown in the AWS IoT console next to the REST endpoint - they're the same.
        /// </summary>
        private const string IotEndpoint = "**********.iot.eu-west-1.amazonaws.com";
        /// <summary>
        /// This is the default TLS1.2 port that AWS IoT uses
        /// </summary>
        private const int BrokerPort = 8883;

        /// <summary>
        /// Just build it and run it up from the bin folder before you publish a message using the publisher
        /// </summary>
        /// <param name="args">expects Nowt</param>
        public static void Main(string[] args)
        {
            var subscriber = new Program();
            subscriber.Subscribe();
        }

        /// <summary>
        /// Set up the client and listen for inbound messages
        /// </summary>
        public void Subscribe()
        {
            //convert to pfx using openssl
            //you'll need to add these two files to the project and copy them to the output
            var clientCert = new X509Certificate2("YOURPFXFILE.pfx", "YOURPFXFILEPASSWORD");
            //this is the AWS caroot.pem file that you get as part of the download
            var caCert = X509Certificate.CreateFromSignedFile("root.pem"); // this doesn't have to be a new X509 type...

            var client = new MqttClient(IotEndpoint, BrokerPort, true, caCert, clientCert, MqttSslProtocols.TLSv1_2 /*this is what AWS IoT uses*/);

            //event handler for inbound messages
            client.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;

            //client id here is totally arbitary, but I'm pretty sure you can't have more than one client named the same.
            client.Connect("listener");

            // '#' is the wildcard to subscribe to anything under the 'root' topic
            // the QOS level here - I only partially understand why it has to be this level - it didn't seem to work at anything else.
            client.Subscribe(new[] { "YOURTHING/#" }, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            while (true)
            {
                //listen good!
            }

        }

        public static void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Debug.WriteLine("We received a message...");
            Debug.WriteLine(Encoding.UTF8.GetChars(e.Message));
        }
    }
}