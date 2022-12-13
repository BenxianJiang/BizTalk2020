using log4net;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ben.Demo.BizTalk.Components
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [System.Runtime.InteropServices.Guid("EF70D7EF-FD8C-4B56-8BFD-886379F50C8B")]
    public class DemoXmlSchemaResolver : IBaseComponent, IPersistPropertyBag, IComponentUI, IDisassemblerComponent, IProbeMessage
    {
        /// <summary>
        /// Use the default microsoft xml disassembler component
        /// </summary>
        private XmlDasmComp _xmlDissambler = new XmlDasmComp();
        private ILog _logger;
        private string _docSpecQN; // = "ES.FS.WG.STAX.Schemas.SAMSchemas";
        private string _assemblyStrongName; // = "ES.FS.WG.STAX.Schemas, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6460ce8afd87a45a";
        private string _docSpecClassName; // = "SamServiceService_servlet_x2esam_x2eedscs_x2eeds_x2ecom";
        private bool _multiElementSchema;

        #region Configuration Properties set via the UI or propertyBag

        public string DocSpecQN
        {
            get { return _docSpecQN; }
            set { _docSpecQN = value; }
        }

        public string AssemblyStrongName
        {
            get { return _assemblyStrongName; }
            set { _assemblyStrongName = value; }
        }

        public string DocSpecClassName
        {
            get { return _docSpecClassName; }
            set { _docSpecClassName = value; }
        }

        public string MultiElementSchema
        {
            get { return _multiElementSchema.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    bool multiElement = true;
                    bool.TryParse(value, out multiElement);

                    _multiElementSchema = multiElement;
                }
            }
        }

        #endregion

        //We only use about 5 or 6 of the SAM services, cache the doc specs locally to avoid roundtripping to biztalk
        private static Dictionary<string, string> StrongNameCache;
        private const int CacheCapacity = 20;

        public DemoXmlSchemaResolver()
        {
            log4net.Ext.Serializable.SLog sLogger = log4net.Ext.Serializable.SLogManager.GetLogger("Ben.Demo.BizTalk", log4net.helpers.CallersTypeName.Name);
            sLogger.RegistryConfigurator();
            _logger = (ILog)sLogger;

            if (StrongNameCache == null)
            {
                StrongNameCache = new Dictionary<string, string>(CacheCapacity);
            }

        }

        #region IBaseComponent

        public string Description
        {
            get { return "Custom XML Schema Resolver to load specific message types from known Assemblies. Used to avoid the error that occurs when multiple applications consume the same target system schema."; }
        }

        public string Name
        {
            get { return "DemoXmlSchemaResolver"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        #endregion

        #region IPersistPropertyBag

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("35AE28A9-33D6-4199-971D-81D8B7A6D9DD");
        }

        public void InitNew()
        {
            //NOOP;
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            string propval = (string)ReadPropertyBag(propertyBag, "DocSpecQN");
            if (propval != null) _docSpecQN = propval;

            propval = (string)ReadPropertyBag(propertyBag, "AssemblyStrongName");
            if (propval != null) _assemblyStrongName = propval;

            propval = (string)ReadPropertyBag(propertyBag, "DocSpecClassName");
            if (propval != null) _docSpecClassName = propval;

            propval = (string)ReadPropertyBag(propertyBag, "MultiElementSchema");
            if (propval != null) MultiElementSchema = propval;
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object objVal = (object)_docSpecQN;
            propertyBag.Write("DocSpecQN", ref objVal);

            objVal = (object)_assemblyStrongName;
            propertyBag.Write("AssemblyStrongName", ref objVal);

            objVal = (object)_docSpecClassName;
            propertyBag.Write("DocSpecClassName", ref objVal);

            objVal = (object)MultiElementSchema;
            propertyBag.Write("MultiElementSchema", ref objVal);
        }

        /// <summary>
        /// Reads property value from property bag.
        /// </summary>
        /// <param name="pb">Property bag.</param>
        /// <param name="propName">Name of property.</param>
        /// <returns>Value of the property.</returns>
        private static object ReadPropertyBag(IPropertyBag pb, string propName)
        {
            object val = null;
            try
            {
                pb.Read(propName, out val, 0);
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

        #region IDisassemblerComponent

        public void Disassemble(IPipelineContext pContext, Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            _xmlDissambler.Disassemble(pContext, pInMsg);
        }

        public Microsoft.BizTalk.Message.Interop.IBaseMessage GetNext(IPipelineContext pContext)
        {
            return _xmlDissambler.GetNext(pContext);
        }

        #endregion

        #region IProbeMessage

        /// <summary>
        /// Examine the incoming message and determine which document spec name (specifically which strong name) to assign.
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="pInMsg"></param>
        /// <returns></returns>
        public bool Probe(IPipelineContext pContext, Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            bool handled = false;

            //the messagetype is not unique, which is why the biztalk resolver fails. However, the docSpecName IS unique as it contains
            //the FQN, eg the docspecName for the SAM unLockBAResponse is ES.FS.WG.SAM.Schemas.SAMSchemas.SAMServiceSchemas.SamServiceService_servlet_x2esam_x2eedscs_x2eeds_x2ecom+unLockBAResponse
            //Whereas the docSpecName for the STAX unLockBAResponse is ES.FS.WG.STAX.Schemas.SAMSchemas.SamServiceService_servlet_x2esam_x2eedscs_x2eeds_x2ecom+unLockBAResponse.

            string rootNodeName = string.Empty;
            string messageType = GetMessageType(pInMsg, out rootNodeName);

            //Is this a messageType we know how to handle
            //ie does the messageType exist in the schema assembly we are resolving to.

            //NOTE: If the schema contains only a single root node, the docSpecName does NOT include the rootNodeName
            string docSpecName = string.Empty;

            if (_multiElementSchema)
            {
                docSpecName = string.Format("{0}.{1}+{2}", DocSpecQN, DocSpecClassName, rootNodeName);
            }
            else
            {
                docSpecName = string.Format("{0}.{1}", DocSpecQN, DocSpecClassName);
            }

            string docSpecStrongName = string.Format("{0}, {1}", docSpecName, AssemblyStrongName);
            IDocumentSpec docSpec = null;

            try
            {
                //determine if this is a valid doc spec name by loading the doc spec (effectively queries the BT mgt database for us)

                //first check the cache, if not in cache retrieve from Biztalk and place in cache
                //if it is in the cache, it is inherently a valid docSpecStrongName as it must have already been looked up from Biztalk
                //so no need to do anything further, just assign the strong name to the message

                if (!StrongNameCache.ContainsKey(rootNodeName))
                {
                    //look up from Biztalk to ensure strongname is valid, if lookup fails an exception is thrown
                    docSpec = pContext.GetDocumentSpecByName(docSpecStrongName);

                    //if capacity reached, remove the first entry added to the cache.
                    if (StrongNameCache.Count == CacheCapacity)
                    {
                        string keyToRemove = StrongNameCache.ElementAt(0).Key;
                        StrongNameCache.Remove(keyToRemove);
                    }

                    StrongNameCache[rootNodeName] = docSpec.DocSpecStrongName;
                }

                handled = true;

            }
            catch (Exception)
            {
                //handle the exception and return false (not handled by this component)
                _logger.DebugFormat("DemoXmlSchemaResolver:: Probe: Message not being handled {0}, DocSpecName {1}", messageType, docSpecStrongName);
            }

            if (handled)
            {
                pInMsg.Context.Write("DocumentSpecName", "http://schemas.microsoft.com/BizTalk/2003/xmlnorm-properties", docSpecStrongName);
            }
            else
            {
                handled = _xmlDissambler.Probe(pContext, pInMsg);
            }

            return handled;
        }

        /// <summary>
        /// Determine whether the message being probed exists in our assembly
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        //private bool CanHandleMessage(string messageType, string docSpecName)
        //{
        //    bool canHandle = false;

        //    //query the biztalk management database to see if the messageType is defined in our Assembly (actually our docSpecName)

        //    SqlDbHelper.

        //}

        /// <summary>
        /// Read the incoming message to determine the messageType 
        /// </summary>
        /// <param name="inMsg">The Biztalk Message to read</param>
        /// <returns>The messageType (namespace+rootnodename)</returns>
        private string GetMessageType(Microsoft.BizTalk.Message.Interop.IBaseMessage inMsg, out string rootNodeName)
        {
            string messageType = string.Empty;

            //string messageType = (string)inMsg.Context.Read("MessageType", MessageContext.SystemPropertiesNamespace);
            //if (messageType != null)
            //{
            //    return messageType;
            //}
            //read the message body to determine rootnode and namespace
            //Wrap the originalStream in a seekable stream so that position can be rewound to 0 before returning

            var originalStream = inMsg.BodyPart.GetOriginalDataStream();
            Stream seekableStream;

            if (!originalStream.CanSeek)
            {
                seekableStream = new ReadOnlySeekableStream(originalStream);
                inMsg.BodyPart.Data = seekableStream;
            }
            else
            {
                seekableStream = originalStream;
            }

            //Stream that will be set to the xml extracted from the incoming message 
            using (var copiedStream = new MemoryStream())
            {
                seekableStream.CopyTo(copiedStream);
                copiedStream.Position = 0;
                seekableStream.Position = 0;

                XDocument xDoc = XDocument.Load(copiedStream);
                messageType = string.Format("{0}#{1}", xDoc.Root.Name.NamespaceName, xDoc.Root.Name.LocalName);
                rootNodeName = xDoc.Root.Name.LocalName;
            }

            _logger.DebugFormat("DemoXmlSchemaResolver:: GetMessageType: Processing Message, determined message type {0}", messageType);

            return messageType;
        }

        #endregion
    }
}
