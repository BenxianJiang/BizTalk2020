// Copyright 2019, DXC
//
// Developed by DXC Technology Adelaide 2019.
// unpublished work created in 2019. This work is a trade secret
// and unauthorised use or copying is prohibited.
// _______________________________________________________________________
// Project Name		: AIBP - Auto Indexing
// File Name        : CustomXmlValidator.cs
// Description      : Class for Custom XML Validation Pipeline Component
// _______________________________________________________________________
// Date			ChangeID	Developer		        Description
// ==========	==========	=============	        =====================
// 2019-02-19	            Mandar Dharmadhikari 	Initial Release                                             
// _______________________________________________________________________

using System;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using System.Xml.Schema;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;
using Microsoft.BizTalk.Streaming;
using System.Collections.Generic;
using System.Xml.Serialization;
using log4net;
namespace Ben.Demo.BizTalk.Components
{
    /// <summary>
    /// Class for Custom XML Validation Pipeline Component
    /// </summary>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Validate)]
    [System.Runtime.InteropServices.Guid("14092222-1541-43A9-B29F-143A150BDA39")]
    public class CustomXmlValidator : IBaseComponent, IPersistPropertyBag, IComponentUI, IComponent
    {
        //Set this property on the pipeline in order to override the default value
        private string _maxErrorCount = "20";
        private const string MaxErrorCountProperty = "MaxErrorCount";

        private ILog _logger;

        public string MaxErrorCount
        {
            get
            {
                return _maxErrorCount;
            }
            set
            {
                _maxErrorCount = value;
            }
        }

        static ResourceManager resourceManager = new ResourceManager("ES.FS.WG.AIBP.Components.CustomValidator", Assembly.GetExecutingAssembly());

        #region Constructor
        public CustomXmlValidator()
        {
            //Use a different registry key (subkey of normal log4net key) so we can initialize a separate logger without locking all the other logger files
            log4net.Ext.Serializable.SLog sLogger = null; // LogManager.Initialise(Constants.AffiliateApplicationName, Constants.Logger.ReceiveLogger, null);
            _logger = (ILog)sLogger;

        }
        #endregion


        #region IBaseComponent


        /// <summary>
        /// Description of the Component
        /// </summary>
        public string Description
        {
            get { return "BizTalk Pipeline Component To Perform Custom Validation for AIBP solution"; }
        }

        /// <summary>
        /// Name of the Pipeline Component
        /// </summary>
        public string Name
        {
            get { return "ES.FS.WG.AIBP.Components.CustomXMLValidator"; }
        }

        /// <summary>
        /// Version of the Pipeline Component
        /// </summary>
        public string Version
        {
            get { return "1.0"; }
        }

        #endregion

        #region IComponentUI

        public IntPtr Icon
        {
            get { return IntPtr.Zero; }
        }

        public System.Collections.IEnumerator Validate(object projectSystem)
        {
            return null;
        }

        #endregion

        #region IPersistPropertyBag
        /// <summary>
        /// Set the Class If to The GUID matching the GUID used for COM registration
        /// </summary>
        /// <param name="classID">GUID</param>
        public void GetClassID(out Guid classID)
        {
            classID = new Guid("14092222-1541-43A9-B29F-143A150BDA39");
        }

        /// <summary>
        /// No Need to implement
        /// </summary>
        public void InitNew()
        {

        }

        /// <summary>
        /// Load the Properties for the Pipeline Components at the runtime
        /// </summary>
        /// <param name="propertyBag">propertyBag of the incoming message</param>
        /// <param name="errorLog">erros generated if any while loading the properties</param>
        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            var val = (string)ReadPropertyBag(propertyBag, MaxErrorCount);
            if (val != null) _maxErrorCount = val;

        }

        /// <summary>
        /// Save the Properties to the Message Property Bag
        /// </summary>
        /// <param name="propertyBag">Property bag of the incomeing message</param>
        /// <param name="clearDirty">Not Used</param>
        /// <param name="saveAllProperties">Not Used</param>
        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            var val = (object)_maxErrorCount;
            propertyBag.Write("MaxErrorCount", ref val);


        }

        #endregion

        #region Icomponent
        /// <summary>
        /// Execute the Custom Logic To Do the Schema Validation
        /// </summary>
        /// <param name="pContext">Pipeline context</param>
        /// <param name="pInMsg">Input message to the pipeline</param>
        /// <returns>Output message</returns>
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            _logger.Debug("PipelineComponent::XmlValidator: Executing validation pipeline component.");

            int maxErrorCount = 20;

            //Parse the maxErrorCount property value to int
            Int32.TryParse(_maxErrorCount, out maxErrorCount);

            string messageType = Convert.ToString(pInMsg.Context.Read(Constants.MessageTypePropName, Constants.SystemPropertiesNamespace));
            string messageId = Convert.ToString(pInMsg.MessageID);
            string fileName = Convert.ToString(pInMsg.Context.Read(Constants.ReceivedFileNamePropName, Constants.FileAdapterPropertiesNameSpace));
            fileName = PipelineHelper.GetFileNameWithoutExtension(fileName);

            XmlValidatorHelper helper = new XmlValidatorHelper(); // (maxErrorCount, fileName);
            helper.Logger = _logger;

            if (pContext == null)
            {
                throw new ArgumentNullException("Pipeline Context is null");
            }

            if (pInMsg == null)
            {
                throw new ArgumentNullException("Incoming Message in null");
            }

            //Create OutMessage

            //Invoke the XMLValidator Validate Method

            var originalStream = pInMsg.BodyPart.GetOriginalDataStream();
            Stream seekableStream;

            if (!originalStream.CanSeek)
            {
                seekableStream = new ReadOnlySeekableStream(originalStream);
                pInMsg.BodyPart.Data = seekableStream;
            }
            else
            {
                seekableStream = originalStream;
            }

            IDocumentSpec docSpec = null;

            try
            {
                docSpec = pContext.GetDocumentSpecByType(messageType);


                //Use a temperory Disposable Virtual Stream
                using (VirtualStream virtusalStream = new VirtualStream())
                {
                    seekableStream.CopyTo(virtusalStream);
                    virtusalStream.Position = 0;
                    helper.ValidationWrapper(docSpec, maxErrorCount, virtusalStream, messageType, messageId);
                }


                //var errorProcessStatus = null; // helper.ProcessStatus;
                ////Check if validation Helper has caught any errors
                //if (errorProcessStatus.Errors != null && errorProcessStatus.Errors.Count() > 0)
                //{

                //    return CreateOutputMessageWrapper(pContext, pInMsg, errorProcessStatus, fileName);

                //}
                //else
                //{
                //    //Need to move the wrapped seekable stream to beginning and assign to the BodyPart as incoming message is not seekable
                //    seekableStream.Seek(0, SeekOrigin.Begin);
                //    // Track the stream so that it can be disposed when the message is finially finished with
                //    pContext.ResourceTracker.AddResource(pInMsg.BodyPart.Data);
                return pInMsg;
                //}
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("RequestId: {0} ErrorMessage: {1}", fileName, ex.Message);
                _logger.Error(errorMessage);
                return pInMsg;

            }

        }





        #endregion


        #region CommonMethods

        /// <summary>
        /// Reads a particular property from the Message Property Bag
        /// </summary>
        /// <param name="pb">Property Bag of the Incoming message</param>
        /// <param name="propertyName">Name of the property to be read</param>
        /// <returns>Object containing the Value of the property</returns>
        private static object ReadPropertyBag(IPropertyBag pb, string propertyName)
        {
            object val = null;

            try
            {
                pb.Read(propertyName, out val, 0);
            }
            catch (ArgumentException)
            {
                return val;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return val;
        }

        /// <summary>
        /// Write the property to the Property Bag
        /// </summary>
        /// <param name="pb">Property Bag of Incoming Message</param>
        /// <param name="val">Value to be written</param>
        /// <param name="propertyName">Name of the property to be written</param>
        private static void WriteToPropertyBag(IPropertyBag pb, object val, string propertyName)
        {
            pb.Write(propertyName, ref val);

        }

        /// <summary>
        /// Wrapper Method To Return the Output message in case of schema Validation Error
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pInMsg"></param>
        /// <param name="processStatus"></param>
        /// <returns></returns>
        private static IBaseMessage CreateOutputMessageWrapper(IPipelineContext pContext, IBaseMessage pInMsg, object processStatus, string fileName)
        {
            VirtualStream errorStream = PipelineHelper.SerializeMessageToXml<object>(processStatus, Constants.AIBPValidationErrorNameSpace);

            IBaseMessage pOutMsg = PipelineHelper.CreateOutPutMessage(pContext, pInMsg, errorStream, fileName);

            return pOutMsg;
        }

        

        #endregion


    }
}
