using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ben.Demo.BizTalk.Components
{
    class SampleCustomPipeline
    {
        /// <summary>
        /// Output new IBaseMessage output from a file.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pInMsg"></param>
        /// <returns></returns>
        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(IPipelineContext pContext, Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            IBaseMessagePart bodyPart = pInMsg.BodyPart;
            Stream inboundStream = bodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(0x280, 0x100000);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream, 0x280);
            string tempFile = Path.GetTempFileName();

            using (FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                byte[] buffer = new byte[0x280];
                int bytesRead = readOnlySeekableStream.Read(buffer, 0, buffer.Length);
                while (bytesRead != 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    fs.Flush();
                    bytesRead = readOnlySeekableStream.Read(buffer, 0, buffer.Length);
                }
            }

            VirtualStream outputStream = new VirtualStream();

            using (XmlWriter xw = XmlWriter.Create(outputStream))
            {
                const string NameSpace = "http://Codit.LFT.Schemas";
                xw.WriteStartDocument();
                xw.WriteStartElement("ns0", "LFT", NameSpace);
                xw.WriteElementString("TempFile", tempFile);
                xw.WriteEndDocument();
            }

            outputStream.Position = 0;
            pContext.ResourceTracker.AddResource(outputStream);
            pInMsg.BodyPart.Data = outputStream;
            return pInMsg;
        }

        /// <summary>
        /// ReadOnlySeekableStream from IBaseMessage
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private ReadOnlySeekableStream GetSeekableStream(IBaseMessage msg)
        {
            Stream inboundStream = msg.BodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream);

            readOnlySeekableStream.Position = 0;
            readOnlySeekableStream.Seek(0, SeekOrigin.Begin);

            return readOnlySeekableStream;
        }

        /// <summary>
        /// Set property from XPath
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pInMsg"></param>
        /// <returns></returns>
        public IBaseMessage ExecuteTwo(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;
            
            //this error Message can be got from validate the message
            //if (!XmlHelper.Validate(out errorMessage))
            //{
            //    throw new ArgumentException(errorMessage);
            //}

            String value = null;

            IBaseMessagePart bodyPart = pInMsg.BodyPart;

            Stream inboundStream = bodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream);

            XmlTextReader xmlTextReader = new XmlTextReader(readOnlySeekableStream);
            //XPathCollection xPathCollection = new XPathCollection();
            //XPathReader xPathReader = new XPathReader(xmlTextReader, xPathCollection);
            //xPathCollection.Add(XPath);

            //while (xPathReader.ReadUntilMatch())
            //{
            //    if (xPathReader.Match(0))
            //    {
            //        value = xPathReader.ReadString();

            //        //if (PromoteProperty)
            //        //{
            //        //    pInMsg.Context.Promote(new ContextProperty(PropertyPath), value);
            //        //}
            //        //else
            //        //{
            //        //    pInMsg.Context.Write(new ContextProperty(PropertyPath), value);
            //        //}

            //        //break;
            //    }
            //}

            //if (string.IsNullOrEmpty(value) && ThrowIfNoMatch)
            //{
            //    throw new InvalidOperationException("The specified XPath did not exist or contained an empty value.");
            //}

            readOnlySeekableStream.Position = 0;
            pContext.ResourceTracker.AddResource(readOnlySeekableStream);
            bodyPart.Data = readOnlySeekableStream;

            return pInMsg;
        }

        public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            //if (!Validate(out errorMessage))
            //{
            //    throw new ArgumentException(errorMessage);
            //}

            //Get a reference to the BizTalk schema.
            string DocumentSpecName = "ProviderRequest";
            var documentSpec = pContext.GetDocumentSpecByName(DocumentSpecName);

            //Get a list of properties defined in the schema.
            var annotations = documentSpec.GetPropertyAnnotationEnumerator();
            var doc = new XmlDocument();
            using (var sw = new StringWriter(new StringBuilder()))
            {
                //Create a new instance of the schema.
                doc.Load(((IFFDocumentSpec)documentSpec).CreateXmlInstance(sw));
            }

            //Write all properties to the message body.
            while (annotations.MoveNext())
            {
                //var annotation = (IPropertyAnnotation)annotations.Current;
                //var node = doc.SelectSingleNode(annotation.XPath);
                //object propertyValue;

                //if (pInMsg.Context.TryRead(new ContextProperty(annotation.Name, annotation.Namespace), out propertyValue))
                //{
                //    node.InnerText = propertyValue.ToString();
                //}
            }

            var data = new VirtualStream();
            pContext.ResourceTracker.AddResource(data);
            doc.Save(data);
            data.Seek(0, SeekOrigin.Begin);

            var outMsg = pInMsg;
            outMsg.BodyPart.Data = data;

            //Promote message type and SchemaStrongName
            //outMsg.Context.Promote(new ContextProperty(SystemProperties.MessageType), documentSpec.DocType);
            //outMsg.Context.Promote(new ContextProperty(SystemProperties.SchemaStrongName), documentSpec.DocSpecStrongName);

            //_outputQueue.Enqueue(outMsg);
        }

        public Microsoft.BizTalk.Message.Interop.IBaseMessage ExecuteThree(IPipelineContext pContext, Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            IBaseMessagePart bodyPart = pInMsg.BodyPart;
            Stream inboundStream = bodyPart.GetOriginalDataStream();
            VirtualStream virtualStream = new VirtualStream(0x280, 0x100000);
            ReadOnlySeekableStream readOnlySeekableStream = new ReadOnlySeekableStream(inboundStream, virtualStream, 0x280);
            XmlTextReader xmlTextReader = new XmlTextReader(readOnlySeekableStream);
            //XPathCollection xPathCollection = new XPathCollection();
            //xPathCollection.Add("/*[local-name()='LFT' and namespace-uri()='http://Codit.LFT.Schemas']/*[local-name()='TempFile' and namespace-uri()='']");
            //XPathReader xPathReader = new XPathReader(xmlTextReader, xPathCollection);
            bool ok = false;
            string val = string.Empty;
            //while (xPathReader.ReadUntilMatch())
            //{
            //    if (xPathReader.Match(0) && !ok)
            //    {
            //        val = xPathReader.ReadString();
            //        ok = true;
            //    }
            //}
            if (ok)
            {
                VirtualStream outboundStream = new VirtualStream(0x280, 0xA00000);
                using (FileStream fs = new FileStream(val, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = fs.Read(buffer, 0, buffer.Length);
                    while (bytesRead != 0)
                    {
                        outboundStream.Write(buffer, 0, bytesRead);
                        outboundStream.Flush();
                        bytesRead = fs.Read(buffer, 0, buffer.Length);
                    }
                }
                outboundStream.Position = 0;
                bodyPart.Data = outboundStream;
            }

            return pInMsg;
        }

        private static void WriteMessage(Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg, XmlDocument xmlDoc)
        {
            var outputStream = new VirtualStream();

            using (var writer = XmlWriter.Create(outputStream, new XmlWriterSettings()
            {
                CloseOutput = false,
                Encoding = Encoding.UTF8
            }))
            {
                xmlDoc.WriteTo(writer);
                writer.Flush();
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            inmsg.BodyPart.Charset = Encoding.UTF8.WebName;
            inmsg.BodyPart.Data = outputStream;
        }

        private static Stream GetSeekeableMessageStream(IBaseMessage message)
        {
            var messageStream = message.BodyPart.GetOriginalDataStream();
            if (messageStream.CanSeek) return messageStream;
            // Create a virtual and seekable stream
            const int bufferSize = 0x280;
            const int thresholdSize = 0x100000;
            Stream virtualReadStream = new VirtualStream(bufferSize, thresholdSize);
            Stream seekableReadStream = new ReadOnlySeekableStream(messageStream, virtualReadStream, bufferSize);
            messageStream = seekableReadStream;
            message.BodyPart.Data = messageStream;
            return messageStream;
        }

        public Stream MessageWriter(Stream stream, Encoding encoding, string nsToModify, string newNamespace, string xPath)
        {
            var outStream = new VirtualStream();

            using (var reader = XmlReader.Create(stream))
            {
                using (var writer = XmlWriter.Create(outStream, new XmlWriterSettings { Encoding = encoding }))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.CDATA:
                                writer.WriteCData(reader.Value);
                                break;

                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;

                            case XmlNodeType.DocumentType:
                                writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"),
                                    reader.GetAttribute("SYSTEM"), reader.Value);
                                break;

                            case XmlNodeType.Element:
                                var isEmpty = reader.IsEmptyElement;

                                // Will call the injected action depending if it's add, modify or remove
                                //writeElement(writer, reader, nsToModify, newNamespace, namespaceForm, xPath);

                                while (reader.MoveToNextAttribute())
                                {
                                    // Copy all attributed that aren't namespaces
                                    if (reader.Value != nsToModify &&
                                        reader.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                                        writer.WriteAttributeString(reader.Prefix, reader.LocalName, reader.NamespaceURI,
                                            reader.Value);
                                }

                                if (isEmpty)
                                    writer.WriteEndElement();

                                break;

                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;

                            case XmlNodeType.EntityReference:
                                writer.WriteEntityRef(reader.Name);
                                break;

                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;

                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;

                            case XmlNodeType.SignificantWhitespace:
                            case XmlNodeType.Whitespace:
                                writer.WriteWhitespace(reader.Value);
                                break;
                        }
                    }
                }
            }

            stream.Seek(0, SeekOrigin.Begin);
            outStream.Seek(0, SeekOrigin.Begin);

            return outStream;
        }

        /// <summary>
        /// Implements IComponent.Execute method.
        /// </summary>
        /// <param name="pc">Pipeline context</param>
        /// <param name="inmsg">Input message</param>
        /// <returns>Original input message</returns>
        /// <remarks>
        /// IComponent.Execute method is used to initiate
        /// the processing of the message in this pipeline component.
        /// </remarks>
        public Microsoft.BizTalk.Message.Interop.IBaseMessage ExecuteFour(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {

            Stream dataStream = new VirtualStream(VirtualStream.MemoryFlag.AutoOverFlowToDisk);
            pc.ResourceTracker.AddResource(dataStream);

            using (XmlWriter writer = XmlWriter.Create(dataStream))
            {

                // Start creating the message body
                writer.WriteStartDocument();
                //writer.WriteStartElement("ns0", ROOT_NODE_NAME, TARGET_NAMESPACE);

                for (int i = 0; i < inmsg.Context.CountProperties; i++)
                {

                    // Read in current property information
                    string propName = null;
                    string propNamespace = null;
                    object propValue = inmsg.Context.ReadAt(i, out propName, out propNamespace);

                    // Skip properties that we don't care about due to configuration (default is to allow all properties)
                    //if (ExcludeSystemProperties && propNamespace == SYSTEM_NAMESPACE) continue;

                    //if (!String.IsNullOrWhiteSpace(CustomPropertyNamespace) &&
                    //        propNamespace != CustomPropertyNamespace) continue;

                    // Create Property element
                    //writer.WriteStartElement(PROPERTY_NODE_NAME);

                    //// Create attributes on Property element
                    //writer.WriteStartAttribute(NAMESPACE_ATTRIBUTE);
                    //writer.WriteString(propNamespace);
                    //writer.WriteEndAttribute();

                    //writer.WriteStartAttribute(NAME_ATTRIBUTE);
                    writer.WriteString(propName);
                    writer.WriteEndAttribute();

                    // Write value inside property element
                    writer.WriteString(Convert.ToString(propValue));

                    writer.WriteEndElement();
                }

                // Finish out the message
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();

            }

            dataStream.Seek(0, SeekOrigin.Begin);

            IBaseMessage outmsg = null;// = Utility.CloneMessage(inmsg, pc);
            //outmsg.BodyPart.Data = dataStream;

            return outmsg;
        }

        /// <summary>
        /// Implements IComponent.Execute method.
        /// </summary>
        /// <param name="pc">Pipeline context</param>
        /// <param name="inmsg">Input message</param>
        /// <returns>Original input message</returns>
        /// <remarks>
        /// IComponent.Execute method is used to initiate
        /// the processing of the message in this pipeline component.
        /// </remarks>
        public Microsoft.BizTalk.Message.Interop.IBaseMessage ExecuteFive(Microsoft.BizTalk.Component.Interop.IPipelineContext pc, Microsoft.BizTalk.Message.Interop.IBaseMessage inmsg)
        {

            #region Handle CORS Requests

            // Detect of the incoming message is an HTTP CORS request
            // http://www.w3.org/TR/cors/

            object httpMethod = null;
            //httpMethod = inmsg.Context.Read(HTTP_METHOD_PROPNAME, WCF_PROPERTIES_NS);

            //if (httpMethod != null && (httpMethod as string) == OPTIONS_METHOD)
            //{
            //    // Remove the message body before returning
            //    var emptyOutputStream = new VirtualStream();
            //    inmsg.BodyPart.Data = emptyOutputStream;

            //    return inmsg;
            //}

            #endregion


            // Make message seekable
            if (!inmsg.BodyPart.Data.CanSeek)
            {
                var originalStream = inmsg.BodyPart.Data;
                Stream seekableStream = new ReadOnlySeekableStream(originalStream);
                inmsg.BodyPart.Data = seekableStream;
                pc.ResourceTracker.AddResource(originalStream);
            }

            // Here again we are loading the entire document into memory
            // this is still a bad plan, and shouldn't be done in production
            // if you expect larger message sizes

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(inmsg.BodyPart.Data);

            if (xmlDoc.FirstChild.LocalName == "xml")
                xmlDoc.RemoveChild(xmlDoc.FirstChild);

            // Remove any root-level attributes added in the process of creating the XML
            // (Think xmlns attributes that have no meaning in JSON)

            xmlDoc.DocumentElement.Attributes.RemoveAll();

            string jsonString = ""; // JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented, true);

            #region Handle JSONP Request

            // Here we are detecting if there has been any value promoted to the jsonp callback property
            // which will contain the name of the function that should be passed the JSON data returned
            // by the service.

            object jsonpCallback = null; // inmsg.Context.Read(JSONP_CALLBACK_PROPNAME, JSON_SCHEMAS_NS);
            string jsonpCallbackName = (jsonpCallback ?? (object)string.Empty) as string;

            if (!string.IsNullOrWhiteSpace(jsonpCallbackName))
                jsonString = string.Format("{0}({1});", jsonpCallbackName, jsonString);

            #endregion

            var outputStream = new VirtualStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)));
            inmsg.BodyPart.Data = outputStream;

            return inmsg;
        }
    }
}
