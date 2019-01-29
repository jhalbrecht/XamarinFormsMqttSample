using System.Threading.Tasks;
namespace MqttDataService
{
    public interface IMqttDataService
    {
        Task Initialize();
        void PublishMqttMessage(string publishmessage);
    }
}
