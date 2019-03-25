using MqttChatApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttChatApp.iOS.services
{
    public class GetCertificatesIos : IGetCertificates
    {
        public void DebugWrite(string text)
        {
            Debug.WriteLine($"\n\nGetCertificatesIos says; {text}\n\n");
        }

        public void GetKey()
        {
            Debug.WriteLine($"\n\nGetCertificatesIos GetKey() says; NOT implemented.\n\n");
        }
    }
}
