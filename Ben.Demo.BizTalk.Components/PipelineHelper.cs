// Copyright 2019, DXC
//
// Developed by DXC Technology Adelaide 2019.
// unpublished work created in 2019. This work is a trade secret
// and unauthorised use or copying is prohibited.
// _______________________________________________________________________
// Project Name		: AIBP - Auto Indexing
// File Name        : PipelineHelper.cs
// Description      : Class to contain Pipeline Helper Methods
// _______________________________________________________________________
// Date			ChangeID	Developer		        Description
// ==========	==========	=============	        =====================
// 2019-02-19	            Mandar Dharmadhikari 	Initial Release                                             
// _______________________________________________________________________

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Streaming;
using Microsoft.XLANGs.BaseTypes;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;

namespace Ben.Demo.BizTalk.Components
{
    /// <summary>
    /// Class to Contain Helper Methods used in various Pipeline Components
    /// </summary>
    public class PipelineHelper
    {
        /// <summary>
        /// Create the Validation Error Message
        /// </summary>
        /// <param name="provider">Stream for the input message</param>
        /// <param name="valErrors">Stream for the captured Validation errors</param>
        /// <returns>Stream for the Validation Error Wrapper message</returns>
        public static VirtualStream CreateValidationErrorMessage(Stream provider, Stream valErrors, string requestId)
        {
            XElement xProvider = XElement.Load(provider);
            XElement xValErrors = XElement.Load(valErrors);           
            //The XValErrors some of the elements in the             
            XNamespace xns = Constants.AIBPValidationErrorNameSpace;
            XElement xAIBPValErrors = new XElement(xns + Constants.AIBPValidationErrorRootNode);
            XElement xAIBPProviderData = new XElement(xns + Constants.AIBPValidationProviderDataNode);            
            //xAIBPValErrors.Add(xProvider);            
            xAIBPProviderData.Add(xProvider);
            xAIBPProviderData.Add(new XElement(xns + Constants.AIBPValidationProviderRequestIDNode, requestId));
            xAIBPValErrors.Add(xAIBPProviderData);
            xAIBPValErrors.Add(xValErrors);

            VirtualStream retStream = new VirtualStream();
            xAIBPValErrors.Save(retStream);
            return retStream;
        }


        /// <summary>
        /// Serialize the Message to Xml
        /// </summary>
        /// <typeparam name="T">Type of the Object to be serialized from </typeparam>
        /// <param name="obj">Object to be Serialized</param>
        /// <returns>Stream of the serialized object</returns>
        public static VirtualStream SerializeMessageToXml<T>(T obj, string nameSpace)
        {
            VirtualStream stream = new VirtualStream();
            XmlSerializer serializer = new XmlSerializer(typeof(T), nameSpace);
            serializer.Serialize(stream, obj);
            return stream;
        }

        /// <summary>
        /// Create the output message when validation errors are captured
        /// </summary>
        /// <param name="pContext">Pipeline context</param>
        /// <param name="pInMsg">Input message in  the pipeline</param>
        /// <param name="errorStream">Stream for the Validation Errors</param>
        /// <param name="requestId">Request Id</param>
        /// <returns></returns>
        public static IBaseMessage CreateOutPutMessage(IPipelineContext pContext, IBaseMessage pInMsg, VirtualStream errorStream, string requestId)
        {
            VirtualStream seekableStream = new VirtualStream(pInMsg.BodyPart.GetOriginalDataStream());
            seekableStream.Position = 0;
            errorStream.Position = 0;
            VirtualStream outMsgStream = CreateValidationErrorMessage(seekableStream, errorStream, requestId);
            outMsgStream.Position = 0;
            
            IBaseMessageFactory messageFactory = pContext.GetMessageFactory();
            IBaseMessage pOutMsg = messageFactory.CreateMessage();
            IBaseMessagePart pOutMsgBodyPart = messageFactory.CreateMessagePart();
            IBasePropertyBag pOutPb = PipelineUtil.CopyPropertyBag(pInMsg.BodyPart.PartProperties, messageFactory);
            pOutMsg.Context = PipelineUtil.CloneMessageContext(pInMsg.Context);
            
            pOutMsgBodyPart.Charset = Constants.BodyPartCharSet;
            pOutMsgBodyPart.ContentType = Constants.BodyPartContentType;
            pOutMsgBodyPart.Data = outMsgStream;
            string outMessageType = string.Format("{0}#{1}", Constants.AIBPValidationErrorNameSpace, Constants.AIBPValidationErrorRootNode);
            pOutMsg.Context.Promote(Constants.MessageTypePropName, Constants.SystemPropertiesNamespace, outMessageType);

            pOutMsg.AddPart("Body", pOutMsgBodyPart, true);

            // Add resources to the resource tracker to be disposed of at correct time
            pContext.ResourceTracker.AddResource(seekableStream);
            pContext.ResourceTracker.AddResource(outMsgStream);
            pContext.ResourceTracker.AddResource(errorStream);
            return pOutMsg;
        }

        /// <summary>
        /// Get File Name without Extension
        /// </summary>
        /// <param name="fileName">File Name with Extension</param>
        /// <returns>File name without extension</returns>
        public static string GetFileNameWithoutExtension(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
