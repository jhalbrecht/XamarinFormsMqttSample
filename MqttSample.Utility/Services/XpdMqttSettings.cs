using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace MqttSample.Utility.Services
{
    public class XpdSettings : IXpdSettings
    {
        /// <summary>
        /// get / set Preferences Mqtt Broker Port
        /// </summary>
        public string MqttBrokerAddress
        {
            get => Preferences.Get(nameof(MqttBrokerAddress), "192.168.1.225");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerAddress), value);
        }
        public string MqttBrokerPort
        {
            get => Preferences.Get(nameof(MqttBrokerPort), "1883");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerPort), value);
        }

        public string MqttBrokerTlsPort
        {
            get => Preferences.Get(nameof(MqttBrokerTlsPort), "8883");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerTlsPort), value);
        }

        public bool UseTls
        {
            get => Preferences.Get(nameof(UseTls), false);
            set => Preferences.Set(nameof(XpdSettings.UseTls), value);
        }
        public string MqttBrokerTopic
        {
            get => Preferences.Get(nameof(MqttBrokerTopic), "test");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerTopic), value);
        }
        public string MqttBrokerUserName
        {
            get => Preferences.Get(nameof(MqttBrokerUserName), "jeffa");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerUserName), value);
        }
        public string MqttBrokerUserPassword
        {
            get => Preferences.Get(nameof(MqttBrokerUserPassword), "burlfloor4");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerUserPassword), value);
        }
    }
}
