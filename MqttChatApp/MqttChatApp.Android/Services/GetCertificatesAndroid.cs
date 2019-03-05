using MqttChatApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttChatApp.Droid.Services
{
    public class GetCertificatesAndroid : IGetCertificates
    {
        void IGetCertificates.DebugWrite(string text)
        {
            Debug.WriteLine($"\n\nGetCertificatesAndroid says; {text}\n\n");
        }
    }
}
