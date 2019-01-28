using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Messaging.Models;
using System.Diagnostics;
using MqttDataService;
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

            _eventAggregator.GetEvent<MessageSentEvent>().Subscribe(TestMessageReceived, ThreadOption.UIThread);
        }

        private ObservableCollection<string> _mqttMessages = new ObservableCollection<string>();
        public ObservableCollection<string> MqttMessages
        {
            get { return _mqttMessages; }
            set
            {
                _mqttMessages = value;
                OnPropertyChanged(nameof(MqttMessages));
            }
        }

        private void TestMessageReceived(string obj)
        {
            MqttMessages.Add(obj);
            Debug.WriteLine($"A IEventAggregator message was received in MqttViewViewModel: {obj}");
        }
    }
}
