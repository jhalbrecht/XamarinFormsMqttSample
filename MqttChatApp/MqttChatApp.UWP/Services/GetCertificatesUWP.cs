using MqttChatApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttChatApp.UWP.Services
{
    public class GetCertificatesUWP : IGetCertificates
    {
        void IGetCertificates.DebugWrite(string text)
        {
            Debug.WriteLine($"\n\nGetCertificatesUWP says; {text}\n\n");
        }
    }
}
