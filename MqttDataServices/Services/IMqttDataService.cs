using System.Threading.Tasks;
namespace MqttDataServices.Services
{
    public interface IMqttDataService
    {
        Task Initialize();
        Task PublishMqttMessageAsync(string publishmessage);
        //Task FilePicker();
        //Task DoLoadCa();
    }
}
