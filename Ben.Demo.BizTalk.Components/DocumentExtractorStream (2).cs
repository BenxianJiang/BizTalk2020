using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Streaming;
using System.IO;
using System.Xml;

namespace Ben.Demo.BizTalk.Components
{
    public class DocumentExtractorStream : XmlTranslatorStream
    {
        private XmlReader reader;
        private XmlWriter writer;
        private string _fileExtension;
        private string _filePath;
        private string _nodeName;

        public DocumentExtractorStream(XmlReader xReader, string filePath, string nodeName, string fileExtension)
            : base(xReader)
        {
            _filePath = filePath;
            _nodeName = nodeName;
            _fileExtension = fileExtension;
            Initialise();
        }

        private void Initialise()
        {
            reader = m_reader;
            writer = m_writer;
        }

        protected override void TranslateStartElement(string prefix, string localName, string nsURI)
        {
            if (reader.LocalName == _nodeName)
            {
                var doc = reader.ReadSubtree();
                doc.Read();
                var data = new byte[4096];
                string Guid = System.Guid.NewGuid().ToString();
                var physicalFilePath = Path.Combine(_filePath, Guid + _fileExtension);
                
                //save node base64 content into physical file.
                using (var fileStream = File.Open(physicalFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
                {
                    int readBytes;
                    while ((readBytes = doc.ReadElementContentAsBase64(data, 0, data.Length)) > 0)
                    {
                        fileStream.Write(data, 0, readBytes);
                    }
                    fileStream.Flush();
                }

                //convert physical path into base64
                var pathBytes = System.Text.Encoding.UTF8.GetBytes(physicalFilePath);
                string pathBase64 = System.Convert.ToBase64String(pathBytes);

                //write into node for physical file path as Base64 - this will be used in Send pipeline to injector the content.
                writer.WriteElementString(prefix, _nodeName, nsURI, pathBase64);
            }
            else
            {
                base.TranslateStartElement(prefix, localName, nsURI);
            }
        }

        protected override void TranslateEndElement(bool full)
        {
            if (reader.LocalName == _nodeName)
            {
            }
            else
            {
                base.TranslateEndElement(full);
            }
        }
    }
}
