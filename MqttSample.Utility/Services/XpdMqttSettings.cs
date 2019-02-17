using Plugin.FilePicker.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MqttSample.Utility.Services
{
    public class XpdSettings : IXpdSettings
    {
        /// <summary>
        /// get / set Preferences Mqtt Broker Port
        /// </summary>
        public string MqttBrokerAddress
        {
            get => Preferences.Get(nameof(MqttBrokerAddress), "192.168.1.225");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerAddress), value);
        }
        public string MqttBrokerPort
        {
            get => Preferences.Get(nameof(MqttBrokerPort), "1883");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerPort), value);
        }

        public string MqttBrokerTlsPort
        {
            get => Preferences.Get(nameof(MqttBrokerTlsPort), "8883");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerTlsPort), value);
        }

        public bool UseTls
        {
            get => Preferences.Get(nameof(UseTls), false);
            set => Preferences.Set(nameof(XpdSettings.UseTls), value);
        }
        public string MqttBrokerTopic
        {
            get => Preferences.Get(nameof(MqttBrokerTopic), "xamtest");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerTopic), value);
        }
        public string MqttBrokerUserName
        {
            get => Preferences.Get(nameof(MqttBrokerUserName), "johndoe");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerUserName), value);
        }
        public string MqttBrokerUserPassword
        {
            get => Preferences.Get(nameof(MqttBrokerUserPassword), "rip");
            set => Preferences.Set(nameof(XpdSettings.MqttBrokerUserPassword), value);
        }

        public async Task LoadCa()
        {
            try
            {
                FileData fileData = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);
                string deviceFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
                File.WriteAllText(deviceFileName, contents);

                Debug.WriteLine("File name chosen: " + fileName);
                Debug.WriteLine("File data: " + contents);

                bool doesExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName));
                Debug.WriteLine(doesExist ? "true" : "false");

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        public async Task LoadPfx()
        {
            try
            {
FileData fileData = await Plugin.FilePicker.CrossFilePicker.Current.PickFile();
if (fileData == null)
    return; // user canceled file picking

string fileName = fileData.FileName;

string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

string content = Convert.ToBase64String(fileData.DataArray, 0, fileData.DataArray.Length,
                Base64FormattingOptions.None);

string deviceFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
File.WriteAllText(deviceFileName, content);

                Debug.WriteLine("File name chosen: " + fileName);
                Debug.WriteLine("File data: " + content);

                bool doesExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName));
                Debug.WriteLine(doesExist ? "true" : "false");

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }
    }
}