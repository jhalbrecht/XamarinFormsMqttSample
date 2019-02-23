using System.Threading.Tasks;
namespace MqttDataServices.Services
{
    public interface IMqttDataService
    {
        Task Initialize();
        void PublishMqttMessage(string publishmessage);
        //Task FilePicker();
        //Task DoLoadCa();
    }
}
