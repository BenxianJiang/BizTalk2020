using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ben.Demo.BizTalk.Components
{
    /// <summary>
    /// Use of the Dispose() method exposed by the XLANGMessage parameter before returning from the .NET code 
    /// is particularly important in looping scenarios and long-running orchestrations 
    /// that can accumulate instances of the XLANGMessage object without releasing them over time. 
    /// </summary>
    class SampleUsingStreamXLangMessage
    {
        /// <summary>
        /// To process a message with an XmlReader instance, 
        /// pass the message to .NET code as an XLANGMessage, and retrieve the Part content using XmlReader
        /// </summary>
        /// <param name="message"></param>
        public void ProcessMessage(XLANGMessage message)
        {
            try
            {
                using (XmlReader reader = message[0].RetrieveAs(typeof(XmlReader)) as XmlReader)
                if (reader != null)
                {}
            }
            finally
            {
                message.Dispose();
            }
        }

        /// <summary>
        /// XmlDocument in orchestrations is to retrieve the message as an XML string using XmlDocument.OuterXml()
        /// Or using the following to
        /// retrieve the message as a string using an XLANGMessage variable
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string MessageToString(XLANGMessage message)
        {
            string strResults;
            try
            {
                using (Stream stream = message[0].RetrieveAs(typeof(Stream)) as Stream)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        strResults = reader.ReadToEnd();
                    }
                }
            }
            finally
            {
                message.Dispose();
            }
            return strResults;
        }

        /// <summary>
        /// Retrieve the contents of simple .NET messages into a string 
        /// </summary>
        /// <param name="message"></param>
        public void ConvertToString(XLANGMessage message)
        {
            try
            {
                string content = message[0].RetrieveAs(typeof(string)) as string;
                if (!string.IsNullOrWhiteSpace(content))
                {}
            }
            finally
            {
                message.Dispose();
            }
        }

        /// <summary>
        /// Retrieve the contents of a message into a stream
        /// </summary>
        /// <param name="message"></param>
        /// <param name="bufferSize"></param>
        /// <param name="thresholdSize"></param>
        /// <returns></returns>
        public Stream ProcessRequestReturnStream(XLANGMessage message, int bufferSize, int thresholdSize)
        {
           Stream partStream = null;

           try
            {
                using (VirtualStream virtualStream = new VirtualStream(bufferSize, thresholdSize))
                {
                    using (partStream = (Stream)message[0].RetrieveAs(typeof(Stream)))
                    
                    //Note that when calling this code, if the XmlDocument is quite large, keeping it in a memory with a MemoryStream may have an adverse effect on performance.
                    //In this case, it may be worthwhile to consider an approach that uses a VirtualStream + ReadonlySeekableStream to buffer it to the file system, 
                    //if its size is bigger than the thresholdSize parameter.
                    //Keep in mind that:
                    // - If the message size is smaller than the threshold size, the VirtualStream class buffers the stream to a MemoryStream.
                    // - If the message size is bigger than the threshold size, the VirtualStream class buffers the stream to a temporary file.
                    using (ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(partStream, virtualStream, bufferSize))
                    {
                        using (XmlReader reader = XmlReader.Create(readOnlySeekableStream))
                        {

                        }
                    }
                }         
            }
           catch (Exception ex)
           {
                throw ex;
           }
           finally
           {
              message.Dispose();
           }

            return partStream;
        }

        /// <summary>
        /// Retrieve the contents of a message into a .NET object.
        /// This technique is valid only when messages are small. Otherwise, this approach could incur in a significant overhead 
        /// to de-serialize the actual message into a .NET object
        /// </summary>
        /// <param name="message"></param>
        public void MessageToObject(XLANGMessage message)
        {
            try
            {
                Request request = message[0].RetrieveAs(typeof(Request)) as Request;
                if (request != null)
                { }
            }
            finally
            {
                message.Dispose();
            }
        }
    }

    public class Request
    {
        public string FullName;
        public string Address;
        public DateTime Dob;
    }
}
