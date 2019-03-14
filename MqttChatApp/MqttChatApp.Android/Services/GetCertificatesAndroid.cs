using Android.Runtime;
using Java.Security;
using Javax.Net.Ssl;
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
            //KeyStore certStore = KeyStore.GetInstance("AndroidCAStore");
            KeyStore certStore = KeyStore.GetInstance(KeyStore.DefaultType); // == BKS
            certStore.Load(null);


            // https://forums.xamarin.com/discussion/14938/certificate-pinning-in-monodroid
            var tmf = TrustManagerFactory.GetInstance(TrustManagerFactory.DefaultAlgorithm);
            tmf.Init((KeyStore)null);
            foreach (var itm in tmf.GetTrustManagers())
            {
                var tm = itm.JavaCast<IX509TrustManager>();
            }


            Debug.WriteLine($"\n\nGetCertificatesAndroid says; {text}\n\n");
        }
        void IGetCertificates.GetKey()
        {
            var certStore = KeyStore.GetInstance("AndroidCAStore");
            certStore.Load(null);
        }
    }
}
/*
KeyStore.getDefaultType()

*/
