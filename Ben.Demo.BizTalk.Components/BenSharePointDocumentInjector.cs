using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.BizTalk.Streaming;

namespace Ben.Demo.BizTalk.Components
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    [System.Runtime.InteropServices.Guid("F9F1129D-126C-4A0B-99E8-C0A6948355EC")]
    public class BenSharePointDocumentInjector : IBaseComponent, IComponentUI, IComponent, IPersistPropertyBag
    {
        #region IBaseComponent
        private const string _description =
            "Pipeline component used to attach large files from disk to the Sharepoint message and convert it to Binary format";
        private const string _name = "Ben Sharepoint Document Injector";
        private const string _version = "1.0.0.0";

        log4net.Ext.Serializable.SLog bLogger;
        log4net.helpers.PropertiesCollectionEx bLogProgs;

        public string Description
        {
            get { return _description; }
        }
        public string Name
        {
            get { return _name; }
        }
        public string Version
        {
            get { return _version; }
        }
        #endregion

        #region IComponentUI
        private IntPtr _icon = new IntPtr();
        public IntPtr Icon
        {
            get { return _icon; }
        }
        public System.Collections.IEnumerator Validate(object projectSystem)
        {
            return null;
        }
        #endregion

        #region IPersistPropertyBag
        private string _NodeName;
        
        public string NodeName
        {
            get { return _NodeName; }
            set { _NodeName = value; }
        }

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("F9F1129D-126C-4A0B-99E8-C0A6948355EC");
        }
        public void InitNew()
        {
        }
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            object Val1 = null;
            
            try
            {
                propertyBag.Read("NodeName", out Val1, 0);
                
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error Occured while Reading the Property bag: Error:" + ex.Message);
            }

            if (Val1 != null)
            {
                _NodeName = (string)Val1;
            }


        }
        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object Val1 = (object)_NodeName;
           
            propertyBag.Write("NodeName", ref Val1);
           
        }
        #endregion
        #region Icomponent

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pInMsg"></param>
        /// <returns></returns>
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            //get ben logger
            Core.GetBenLogger(Guid.NewGuid().ToString(), log4net.helpers.CallersTypeName.Name, out bLogger, out bLogProgs);
            bLogger.Debug(bLogProgs, "Start Execute ..." + _NodeName);

            XmlTextReader reader = new XmlTextReader(pInMsg.BodyPart.Data);
            IBaseMessageFactory factory = pContext.GetMessageFactory();
            IBaseMessage message = factory.CreateMessage();
            IBaseMessagePart body = factory.CreateMessagePart();
  
            //Read the Node Name from the Context and Migrate to the node in Message to Read the 
            //File Path and Replace and Read the message content from there to create a binary message
            byte[] resultBytes;
            string docFileValue = GetNodeValue(reader);

            //if (reader.ReadToDescendant(_name)) //reader.ReadToDescendant(_NodeName, Constants.ArticleNodeNameSpace))
            if (!string.IsNullOrWhiteSpace(docFileValue))
            {  
                //reader.Read();  // read to the text
                //string docFileValue = reader.Value;
                bLogger.Debug(bLogProgs, "Node value is " + docFileValue);
                resultBytes = File.ReadAllBytes(docFileValue);    // Get the text value    

                //File.WriteAllBytes(@"c:\temp\newTest.pdf", resultBytes);
            }
            else
            {
                bLogger.Debug(bLogProgs, "Doc file empty!");
                resultBytes = new byte[0];
            }

            MemoryStream memStream = new MemoryStream();
            memStream.Write(resultBytes, 0, resultBytes.Length);
            memStream.Position = 0;
            body.Data = memStream;
            pContext.ResourceTracker.AddResource(memStream);
            StreamReader sr = new StreamReader(body.Data);
            message.AddPart(pInMsg.BodyPartName, body, true);
            message.Context = pInMsg.Context;

            bLogger.Debug(bLogProgs, "End Execute ###");

            return message; 
        }

        /// <summary>
        /// Get node string value decoded from Base64
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private string GetNodeValue(XmlTextReader reader)
        {
            string pathDecoded = string.Empty;

            //must use ReadToFollowing! 
            while (reader.ReadToFollowing(_NodeName))
            {
                string filePath = reader.ReadElementContentAsString();

                //Decoded file path from base64
                byte[] data = System.Convert.FromBase64String(filePath);
                pathDecoded = System.Text.ASCIIEncoding.UTF8.GetString(data);

                //byte[] buffer = new byte[8096];          
                //reader.ReadContentAsBase64(buffer, 0, 8096);
                //string myPath = System.Text.Encoding.UTF8.GetString(buffer); 

                //Now we read the document from the FileSite using the saved Path
                //byte[] pdfBytes = File.ReadAllBytes(pathDecoded);
                //var docData = Convert.ToBase64String(pdfBytes);

                //write back to Node with base64
                //writer.WriteElementString(localName, nsURI, docData);
                bLogger.Debug(bLogProgs, "Path = " + pathDecoded);
                //bLogger.Debug(bLogProgs, "MyPath = " + myPath);
            }      

            return pathDecoded;
        }
        #endregion
    }
}
