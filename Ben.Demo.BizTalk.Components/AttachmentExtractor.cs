using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ben.Demo.BizTalk.Components
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Decoder)]
    [System.Runtime.InteropServices.Guid("6AA5F470-24D3-46B1-90C3-BEABED0ECF54")]
    public class AttachmentExtractor : IBaseComponent, IComponentUI, IComponent, IPersistPropertyBag
    {
        #region IBaseComponent
        private const string _description = "Ben Demo Pipeline component used to save large files to disk";
        private const string _name = "Ben Demo Document Extractor";
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
        private string _UncFilepath;
        private string _FileExtension;

        public string NodeName
        {
            get { return _NodeName; }
            set { _NodeName = value; }
        }

        public string UncFilepath
        {
            get { return _UncFilepath; }
            set { _UncFilepath = value; }
        }

        public string FileExtension
        {
            get { return _FileExtension; }
            set { _FileExtension = value; }
        }

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("6AA5F470-24D3-46B1-90C3-BEABED0ECF54");
        }

        public void InitNew()
        {
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            object Val1 = null;
            object Val2 = null;
            object Val3 = null;

            try
            {
                propertyBag.Read("NodeName", out Val1, 0);
                propertyBag.Read("UncFilepath", out Val2, 0);
                propertyBag.Read("FileExtension", out Val3, 0);
            }
            catch (ArgumentException)
            {
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error While reading the Property Bag. Error:" + ex.Message);
            }

            if (Val1 != null)
            {
                _NodeName = (string)Val1;
            }
            if (Val2 != null)
            {
                _UncFilepath = (string)Val2;
            }
            if (Val3 != null)
            {
                _FileExtension = (string)Val3;
            }
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            object Val1 = (object)_NodeName;
            object Val2 = (object)_UncFilepath;
            object Val3 = (object)_FileExtension;
            propertyBag.Write("NodeName", ref Val1);
            propertyBag.Write("UncFilepath", ref Val2);
            propertyBag.Write("FileExtension", ref Val3);
        }

        #endregion

        #region Icomponent

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            Stream originalStream = pInMsg.BodyPart.GetOriginalDataStream();
            XmlReader xReader = XmlReader.Create(originalStream);
            pInMsg.BodyPart.Data = new DocumentExtractorStream(xReader, _UncFilepath, _NodeName, _FileExtension);

            return pInMsg;           
        }

        #endregion
    }
}
