using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Ben.Demo.WcfService
{
    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Document
    {
        string fileName = string.Empty;
        byte[] content = null;

        [DataMember]
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        [DataMember]
        public byte[] Content
        {
            get { return content; }
            set { content = value; }
        }
    }
}