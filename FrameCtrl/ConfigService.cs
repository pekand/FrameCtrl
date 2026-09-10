using System;
using System.IO;
using System.Xml.Serialization;

namespace FrameCtrl
{
    public class ConfigService
    {
        private readonly string _filePath;
        private readonly string appFolderName = "FrameCtrl";
        public ConfigService()
        {
            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string directoryPath = Path.Combine(roamingPath, appFolderName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            _filePath = Path.Combine(directoryPath, "config.xml");
        }

        public void Save(Config config)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                using (StreamWriter writer = new StreamWriter(_filePath))
                {
                    serializer.Serialize(writer, config);
                }

            }
            catch (Exception)
            {

            }
        }

        public Config Load()
        {
            if (!File.Exists(_filePath))
            {
                return new Config();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    return (Config)serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {

                return new Config();
            }
            
        }
    }
}