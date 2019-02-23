////Client:
//> using MQTTnet.Client;
//using System;
//using System.Security.Cryptography.X509Certificates;

//class Program
//{
//    private static bool remoteValidation(X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors, MqttClientTcpOptions options)
//    { return true; }
//    //{ //check certificate and return true or false return true; }
//    static void Main(string[] args)
//    {
//        testi();
//        Console.ReadLine();
//    }
//    private static async void testi()
//    {
//        var mqttClient = new MqttClientFactory().CreateMqttClient();
//        var _mqttOptions = new MqttClientTcpOptions();
//        _mqttOptions.CleanSession = true;
//        _mqttOptions.ClientId = "TEST";
//        _mqttOptions.DefaultCommunicationTimeout = TimeSpan.FromSeconds(20);
//        _mqttOptions.KeepAlivePeriod = TimeSpan.FromSeconds(31);
//        _mqttOptions.Server = "127.0.0.1";
//        _mqttOptions.UserName = "MQTTUserName";
//        _mqttOptions.Port = 8883;
//        _mqttOptions.TlsOptions = new MqttClientTlsOptions
//        {
//            UseTls = true,
//            AllowUntrustedCertificates = true,
//            IgnoreCertificateChainErrors = true,
//            IgnoreCertificateRevocationErrors = true
//        };
//        MqttTcpChannel.CustomCertificateValidationCallback = remoteValidation;
//        await mqttClient.ConnectAsync(_mqttOptions);
//        mqttClient.Disconnected += async (s, e) =>
//        {
//            Console.WriteLine("### DISCONNECTED FROM SERVER ###");
//            await Task.Delay(TimeSpan.FromSeconds(5));
//            try
//            {
//                await mqttClient.ConnectAsync(_mqttOptions);
//            }
//            catch
//            {
//                Console.WriteLine("### RECONNECTING FAILED ###");
//            }
//        };
//    }
//}

////And this is my Broker:

//public static async void broker()
//{
//    Console.WriteLine("Starting");
//    var options = new MqttServerOptions
//    {
//        ConnectionValidator = p =>
//        {
//            Console.WriteLine("New");
//            Console.WriteLine(p.ClientId);
//            return MqttConnectReturnCode.ConnectionAccepted;
//        }
//    };
//    options.TlsEndpointOptions.IsEnabled = true;       //message will be encrypted
//    options.TlsEndpointOptions.Certificate = new X509Certificate2(File.ReadAllBytes(@"C:\Certs\cert.pem")).RawData;
//    var mqttServer = new MqttServerFactory().CreateMqttServer(options);
//    mqttServer.ApplicationMessageReceived += (s, e) =>
//    {
//        string messageString = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
//        try
//        {
//            Dictionary<string, string> messageProperty = new Dictionary<string, string>();
//            Console.WriteLine(e.ClientId + " sent " + messageString);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex);
//        }
//    };
//    Console.WriteLine("Starting MqttBroker");
//    await mqttServer.StartAsync();
//    Console.WriteLine("MqttBroker started");
//    Console.ReadLine();
//}
