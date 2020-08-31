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
    [System.Runtime.InteropServices.Guid("29FB733E-7CE0-47F4-9007-5313A9FEBF4B")]
    public class AttachmentInjector : IBaseComponent, IComponentUI, IComponent, IPersistPropertyBag
    {
        #region IBaseComponent

        private const string _description = "Ben Demo Pipeline component used to attach large files from disk to the corresponding messages";
        private const string _name = "Ben Demo Document Injector";
        private const string _version = "1.0.0.0";

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
            classID = new Guid("29FB733E-7CE0-47F4-9007-5313A9FEBF4B");
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

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {

            Stream originalStream = pInMsg.BodyPart.GetOriginalDataStream();
            XmlReader xReader = XmlReader.Create(originalStream);
            pInMsg.BodyPart.Data = new DocumentInjectorStream(xReader, _NodeName);

            return pInMsg;
        }

        #endregion
    }
}
