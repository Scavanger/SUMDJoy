﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SUMDJoy
{
    public class Settings
    {
        public VJoyConfig VJoyConfig { get; set; }

        public Settings()
        {

            VJoyConfig = new VJoyConfig();
        }

        public void Load(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("filename");

            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                    DataContractSerializer serializer = new DataContractSerializer(typeof(VJoyConfig));
                    VJoyConfig = serializer.ReadObject(reader, true) as VJoyConfig;
                }
            }
            else
                throw new FileNotFoundException(fileName);
        }

        public void Save(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("filename");

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(VJoyConfig));
                serializer.WriteObject(fs, VJoyConfig);
            }
        }

    }


}
