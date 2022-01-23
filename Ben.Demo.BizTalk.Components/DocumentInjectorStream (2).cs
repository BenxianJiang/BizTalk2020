using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Streaming;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace Ben.Demo.BizTalk.Components
{
    public class DocumentInjectorStream : XmlTranslatorStream
    {
        private string _nodeName;
        private XmlReader reader;
        private XmlWriter writer;

        public DocumentInjectorStream(XmlReader xReader, string nodeName)
            : base(xReader)
        {

            this._nodeName = nodeName;
            Initialize();
        }

        private void Initialize()
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

                string filePath = doc.ReadElementContentAsString();

                //Decoded file path from base64
                byte[] data = System.Convert.FromBase64String(filePath);
                string pathDecoded = System.Text.ASCIIEncoding.UTF8.GetString(data);
                                
                //Now we read the document from the FileSite using the saved Path
                byte[] pdfBytes = File.ReadAllBytes(pathDecoded);
                var docData = Convert.ToBase64String(pdfBytes);

                //write back to Node with base64
                writer.WriteElementString(localName, nsURI, docData);

                // Delete the File after it has been injected into the message
                //try
                //{
                //   File.Delete(pathDecoded);
                //}
                //catch(Exception ex)
                //{
                //    throw new Exception("Can not delete file!" + ex.Message);
                //}
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
