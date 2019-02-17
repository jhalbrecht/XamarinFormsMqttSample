using MqttDataService;
using MqttSample.Utility.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace MqttSample.ViewModels
{
    public class SettingsPageViewModel : BindableBase // TODO I want to use ViewModelBase Brian @brianlagunas di problems...
    {
        private IXpdSettings _xPdSetting;
        private IMqttDataService _mqttDataService;

        public SettingsPageViewModel(IXpdSettings xpdSettings, IMqttDataService mqttDataService)
        {
            Title = "new settings page";
            _xPdSetting = xpdSettings;
            _mqttDataService = mqttDataService;

            LoadCa = new DelegateCommand(() => _xPdSetting.LoadCa());
            LoadPfx = new DelegateCommand(() => _xPdSetting.LoadPfx());
        }

        public ICommand LoadCa { get; set; }
        public ICommand LoadPfx { get; set; }

        public string Title { get; set; }
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
