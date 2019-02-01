using Messaging.Models;
using MqttDataService;
using MqttDataService.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MqttSample.ViewModels
{
    public class MqttViewViewModel : BindableBase
    {
        private IXpdSettings _xpdsettings;
        private IEventAggregator _eventAggregator;
        private IMqttDataService _mqttDataService;

        public MqttViewViewModel(IEventAggregator eventAggregator, IMqttDataService MqttDataService, IXpdSettings xpdSettings)
        {
            _xpdsettings = xpdSettings;
            _eventAggregator = eventAggregator;
            _mqttDataService = MqttDataService;
            Task.Run(async () =>
            {
                await _mqttDataService.Initialize();
            });
            _eventAggregator.GetEvent<MqttMessageTransport>().Subscribe(MqttMessageTransportMessageReceived, ThreadOption.UIThread);
            HotKeyCommandButton = new Command<string>(DoHotKeyCommandButton);
            PublishMessageCommand = new DelegateCommand(ExecutePublishMessageCommand, CanPublish);

            MosquittoPubSub = $"mosquitto_pub -h {_xpdsettings.MqttBrokerAddress} -t {_xpdsettings.MqttBrokerTopic} -m 'Your message goes here.'";
        }

        /// <summary>
        /// PublishMessageCommand
        /// </summary>
        public ICommand PublishMessageCommand { get; set; }
        private void ExecutePublishMessageCommand()
        {
            _mqttDataService.PublishMqttMessage(PublishMessage);
            PublishMessage = string.Empty; // restore the Entry Placeholder
            RaisePropertyChanged("PublishMessage");
        }
        private bool CanPublish()
        {
            return true;
            //if (PublishMessage.Length > 1)
            //    // return System.Func<bool>.Equals = true;
            //    return true;
            //return false;
        }
        //public bool IsMqttPublishButtonEnabled { get; set; };
        private bool _isMqttPublishButtonEnabled = false;

        public bool IsMqttPublishButtonEnabled
        {
            get { return _isMqttPublishButtonEnabled; }
            //set { _isMqttPublishButtonEnabled = value; }
            set
            {
                SetProperty(ref _isMqttPublishButtonEnabled, value);
            }
        }


        /// <summary>
        /// Hot key command
        /// </summary>
        public ICommand HotKeyCommandButton { get; private set; }

        void DoHotKeyCommandButton(string value)
        {
            // Debug.WriteLine($"DoPublishFritzMessageCommand parm value: {value}");

            switch (value)
            {
                case "Fritz":
                    _mqttDataService.PublishMqttMessage($"{value} says; \"And then?\"");
                    break;
                case "James":
                    _mqttDataService.PublishMqttMessage($"{value} says; Monkey see, monkey dodo");
                    break;
                case "TimH":
                    _mqttDataService.PublishMqttMessage(
                        $"{value} says; Bicycle, bicycle, bicycle I want to ride my bicycle, bicycle, bicycle");
                    break;
                case "BrianL":
                    _mqttDataService.PublishMqttMessage(
                        $"{value} says; Prism Prism Xamarin.Forms Prism all the things!");
                    break;
                case "blowdart":
                    _mqttDataService.PublishMqttMessage(
                        $"{value} says; Well did you?");
                    break;
            }
        }

        public string MosquittoPubSub { get; set; }

        private ObservableCollection<MqttMessageTransport> _mqttMessageTransport = new ObservableCollection<MqttMessageTransport>();
        public ObservableCollection<MqttMessageTransport> MqttMessageTransportMessages
        {
            get { return _mqttMessageTransport; }
            set { _mqttMessageTransport = value; }
        }
        public string PublishMessage { get; set; }

        // TODO: Why would I use either DelegateCommand or ICommand over the other ???
        //private DelegateCommand _publishFritzMessageCommand;
        //public DelegateCommand PublishFritzMessageCommand =>
        //    _publishFritzMessageCommand ?? (_publishFritzMessageCommand = new DelegateCommand(ExecutePublishFritzMessageCommand));

        private void MqttMessageTransportMessageReceived(MqttMessageTransport obj)
        {
            MqttMessageTransportMessages.Add(obj);
            Debug.WriteLine($"MqttMessageTransport message received in MqttViewViewModel: Topic: '{obj.Topic}' Message: '{obj.Message}");
        }
    }
}
