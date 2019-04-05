using MqttChattApp.Utility.Services;
using MqttDataServices.Models;
using MqttDataServices.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MqttChatApp.ViewModels
{
    public class ChatPageViewModel : ViewModelBase // BindableBase
    {
        private IXpdSettings _xpdsettings;
        private IEventAggregator _eventAggregator;
        private IMqttDataService _mqttDataService;
        public ChatPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IMqttDataService MqttDataService, IXpdSettings xpdSettings) : base(navigationService)
        {
            Title = "Mqtt Chat";
            _xpdsettings = xpdSettings;
            _eventAggregator = eventAggregator;
            _mqttDataService = MqttDataService;
            Task.Run(async () =>
            {
                await _mqttDataService.Initialize();
            });
            _eventAggregator.GetEvent<MqttMessageTransport>().Subscribe(MqttMessageTransportMessageReceived, ThreadOption.UIThread);
            // HotKeyCommandButton = new Command<string>(DoHotKeyCommandButton);
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
