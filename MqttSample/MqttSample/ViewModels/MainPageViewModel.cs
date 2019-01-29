using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MqttSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Mqtt demo app";
        }

        private DelegateCommand _settingsNavigateCommand;
        public DelegateCommand SettingsNavigateCommand =>
            _settingsNavigateCommand ?? (_settingsNavigateCommand = 
            new DelegateCommand(ExecuteSettingsNavigateCommand));

        async void ExecuteSettingsNavigateCommand()
        {
            await _navigationService.NavigateAsync("SettingsView");
        }

        /// <summary>
        /// MqttNavigateCommand
        /// </summary>
        private DelegateCommand _mqttNavigateCommand;
        public DelegateCommand MqttNavigateCommand =>
            _mqttNavigateCommand ?? (_mqttNavigateCommand = new DelegateCommand(ExecuteMqttNavigateCommand));

        async void ExecuteMqttNavigateCommand()
        {
            await _navigationService.NavigateAsync("MqttView");
        }

        // TODO: ask someone whoe knows.... Could I replace my messaging with an event or ???
        //private void _client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        //{
        //    var message = System.Text.Encoding.Default.GetString(e.Message);
        //    Debug.WriteLine(message);
        //    //_eventAggregator.GetEvent<MessageSentEvent>().Publish(TheTestMessage);
        //}
    }
}
