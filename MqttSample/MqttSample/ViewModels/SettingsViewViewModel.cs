using MqttDataService.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MqttSample.ViewModels
{

    public class SettingsViewViewModel : BindableBase
    {
        public IXpdSettings _xPdSetting;
        public SettingsViewViewModel(IXpdSettings xpdSettings)
        {
            _xPdSetting = xpdSettings;
            // Title = "Mqtt Settings";
        }

        /// <summary>
        /// Mqtt Brokder FQDN or ip address
        /// </summary>
        public string MqttBrokerAddress
        {
            get => _xPdSetting.MqttBrokerAddress;
            set
            {
                _xPdSetting.MqttBrokerAddress = value;
            }
        }

        /// <summary>
        /// Mqtt Broker Port
        /// </summary>
        public string MqttBrokerPort
        {
            get => _xPdSetting.MqttBrokerPort;
            set
            {
                _xPdSetting.MqttBrokerPort = value;
            }
        }

        /// <summary>
        /// Mqtt Broker Port
        /// </summary>
        public string MqttBrokerTlsPort
        {
            get => _xPdSetting.MqttBrokerTlsPort;
            set
            {
                _xPdSetting.MqttBrokerTlsPort = value;
            }
        }

        /// <summary>
        /// Mqtt user name
        /// </summary>
        public string MqttBrokerUserName
        {
            get => _xPdSetting.MqttBrokerUserName;
            set
            {
                _xPdSetting.MqttBrokerUserName = value;
            }
        }

        /// <summary>
        /// Use TLS?
        /// </summary>
        public bool UseTls
        {
            get => _xPdSetting.UseTls;
            set
            {
                _xPdSetting.UseTls = value;
            }
        }
        /// <summary>
        /// mqtt user name password
        /// </summary>
        public string MqttBrokerUserPassword
        {
            get => _xPdSetting.MqttBrokerUserPassword;
            set
            {
                _xPdSetting.MqttBrokerUserPassword = value;
            }
        }
        /// <summary>
        /// Mqtt topic
        /// </summary>
        public string MqttBrokerTopic
        {
            get => _xPdSetting.MqttBrokerTopic;
            set
            {
                _xPdSetting.MqttBrokerTopic = value;
            }
        }

    }
}
