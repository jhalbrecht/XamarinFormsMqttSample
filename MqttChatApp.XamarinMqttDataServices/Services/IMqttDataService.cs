using System.Threading.Tasks;
namespace MqttChatApp.XamarinMqttDataServices.Services
{
    public interface IMqttDataService
    {
        Task Initialize();
        void PublishMqttMessage(string publishmessage);
        //Task FilePicker();
        //Task DoLoadCa();
    }
}
