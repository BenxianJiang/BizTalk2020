using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

namespace WixServiceWinApp
{
    public static class Core
    {
        private const string sampleMessagesDir = @"..\..\SampleMessages";
        private const string canonicalDocumentType = "ES.FS.WG.AIBP.Schemas.AIBPCanonical";

        public static string GetSampleMessagesPath()
        {
            DirectoryInfo dir = new DirectoryInfo(sampleMessagesDir);
            return dir.FullName;
        }

        public static string GetSampleMessagesFolder()
        {
            string dataFolder = @"..\..\..\ES.FS.WG.AIBP.UnitTest\SampleMessages";
            DirectoryInfo dir = new DirectoryInfo(dataFolder);
            string fullName = dir.FullName;

            return fullName;
        }


        public static XDocument GetCanonicalSampleXDocumnet(string dataSubFolderName, string sampleName)
        {
            string path = Path.Combine(GetSampleMessagesPath(), dataSubFolderName, sampleName);
            XDocument xDoc = XDocument.Parse(File.ReadAllText(path));
            return xDoc; ;
        }

        public static XmlDocument GetSampleXml(string dataSubFolderName, string sampleName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            string path = Path.Combine(GetSampleMessagesPath(), dataSubFolderName, sampleName);
            xmlDoc.Load(path);

            return xmlDoc;
        }

        public static string GetTestOutFolder()
        {
            string dataFolder = @"..\..\..\ES.FS.WG.AIBP.UnitTest\TestOut";
            DirectoryInfo dir = new DirectoryInfo(dataFolder);
            string fullName = dir.FullName;

            return fullName;
        }

        public static DateTime GetAustraliaCentralStandardTime()
        {
            string timeZone = "Cen. Australia Standard Time";
            TimeZoneInfo zoneID = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            DateTime nowAtTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zoneID);

            return nowAtTimeZone;
        }

        public static string SerializeToXml(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            StringBuilder result = new StringBuilder();
            using (var writer = XmlWriter.Create(result))
            {
                serializer.Serialize(writer, obj);
            }

            return result.ToString();
        }

        public static WcfWixService.AIBPCanonical DeserializeAIBPCanonicalFromXml(string xml)
        {
            WcfWixService.AIBPCanonical result = null;

            XmlSerializer serializer = new XmlSerializer(typeof (WcfWixService.AIBPCanonical));
           
            using (var stringreader = new StringReader(xml))
            {
                result = (WcfWixService.AIBPCanonical) serializer.Deserialize(stringreader);
            }

            return result;
        }

        public static T DeserializeFromXmlString<T>(string xml)
        {
            T instance;

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var stringreader = new StringReader(xml))
                {
                    instance = (T)xmlSerializer.Deserialize(stringreader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return instance;
        }

        public static T DeserializeFromXmlFile<T>(string xmlFilePath)
        {
            string xml = File.ReadAllText(xmlFilePath);
            return DeserializeFromXmlString<T>(xml);
        }
    }
}
