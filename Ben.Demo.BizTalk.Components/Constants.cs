using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ben.Demo.BizTalk.Components
{
    public class Constants
    {
        public const string AIBPValidationErrorNameSpace = "urn:dxc.esb.aibp.validationerrors/v1";

        public const string AIBPValidationErrorRootNode = "AIBPValidationErrors";
        public const string AIBPValidationProviderDataNode = "ProviderData";
        public const string AIBPValidationProviderRequestIDNode = "RequestID";

        public const string SystemPropertiesNamespace = "http://schemas.microsoft.com/BizTalk/2003/system-properties";

        public const string FileAdapterPropertiesNameSpace = "http://schemas.microsoft.com/BizTalk/2003/file-properties";

        public const string BodyPartCharSet = "UTF-8";

        public const string BodyPartContentType = "text/xml";

        public const string MessageTypePropName = "MessageType";

        public const string ReceivedFileNamePropName = "ReceivedFileName";

        /// <summary>
        /// Name of the ProjectPath setting in SSO settings,
        /// eg. default to "C:\Program Files (x86)\ES.FS.WG.MSAPI for BizTalk\1.0" from SSO settings.
        /// </summary>
        public const string ProjectPath = "ProjectPath";

        /// <summary>
        /// The Common Affiliate Application Name (must be same as "ProjectName" in BTDF configuration).
        /// </summary>
        public const string AffiliateApplicationName = "Ben.Demo.BizTalk";

        /// <summary>
        /// database connection string setting in SSO settings
        /// </summary>
        public const string DbConnectionString = "DbConnectionString";

        /// <summary>
        /// local connection string to BiztalkStore
        /// </summary>
        public const string LocalConnectionString = "Data Source=localhost;Initial Catalog=BiztalkStore;Integrated Security=SSPI;";

        /// <summary>
        /// Server name in SSO
        /// </summary>
        public const string ServerName = "BizTalkStateStore_ServerName";

        /// <summary>
        /// Database name in SSO
        /// </summary>
        public const string DatabaseName = "BizTalkStateStore_DatabaseName";

        /// <summary>
        /// Authentication string setting in SSO
        /// </summary>
        public const string DbAuthentication = "BizTalkStateStore_DbAuthentication";

        /// <summary>
        /// Reindexing path string setting in SSO
        /// </summary>
        public const string ReIndexingDocPath = "ReIndexingLandingZonePath";

        /// <summary>
        /// Response Message file path string setting in SSO
        /// </summary>
        public const string ResponseMessageTemplateFile = "ResponseMessageTemplateFile";

        /// <summary>
        /// Provider namespace
        /// </summary>
        public const string ProviderNamespace = "urn:dxc.esb.aibp.provider/v1";

        /// <summary>
        /// ExternalReferenceID xpath
        /// </summary>
        public const string ExternalReferenceIDPath = "/*[local-name()='ProviderRequest' and namespace-uri()='urn:dxc.esb.aibp.provider/v1']/*[local-name()='ExternalReferenceID' and namespace-uri()='urn:dxc.esb.aibp.provider/v1']";

        /// <summary>
        /// Schema fully qualified name
        /// </summary>
        public const string SchemaFullyQualifiedName = "ES.FS.WG.MSAPI.Schemas, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1637b744db65b335";
    }
}
