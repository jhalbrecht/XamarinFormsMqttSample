using Messaging.Models;
using MqttDataService;
using Prism.Events;
using Prism.Mvvm;
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
        private void MqttMessageTransportMessageReceived(MqttMessageTransport obj)
        {
            MqttMessageTransportMessages.Add(obj);
            Debug.WriteLine($"MqttMessageTransport message received in MqttViewViewModel: Topic: '{obj.Topic}' Message: '{obj.Message}");
        }
    }
}
