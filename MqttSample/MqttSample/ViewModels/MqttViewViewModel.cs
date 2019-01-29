using Messaging.Models;
using MqttDataService;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MqttSample.ViewModels
{
    public class MqttViewViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private IMqttDataService _mqttDataService;

        public MqttViewViewModel(IEventAggregator eventAggregator, IMqttDataService MqttDataService)
        {
            _eventAggregator = eventAggregator;
            _mqttDataService = MqttDataService;
            Task.Run(async () =>
            {
                await _mqttDataService.Initialize();
            });
            _eventAggregator.GetEvent<MqttMessageTransport>().Subscribe(MqttMessageTransportMessageReceived, ThreadOption.UIThread);
        }

        private ObservableCollection<MqttMessageTransport> _mqttMessageTransport = new ObservableCollection<MqttMessageTransport>();
        public ObservableCollection<MqttMessageTransport> MqttMessageTransportMessages
        {
            get { return _mqttMessageTransport; }
            set { _mqttMessageTransport = value; }
        }

        public string PublishMessage { get; set; }

        private DelegateCommand _publishMessageCommand;
        public DelegateCommand PublishMessageCommand =>
            _publishMessageCommand ?? (_publishMessageCommand = new DelegateCommand(ExecutePublishMessageCommand));

        private void ExecutePublishMessageCommand()
        {
            _mqttDataService.PublishMqttMessage(PublishMessage);
            PublishMessage = string.Empty; // restore the Entry Placeholder
            RaisePropertyChanged("PublishMessage"); 
        }

        private void MqttMessageTransportMessageReceived(MqttMessageTransport obj)
        {
            MqttMessageTransportMessages.Add(obj);
            Debug.WriteLine($"MqttMessageTransport message received in MqttViewViewModel: Topic: '{obj.Topic}' Message: '{obj.Message}");
        }
    }
}
