using System.Threading.Tasks;
namespace MqttDataService
{
    public interface IMqttDataService
    {
        Task Initialize();
        void PublishMqttMessage(string publishmessage);
        //Task FilePicker();
        //Task DoLoadCa();
    }
}
