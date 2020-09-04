using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Ben.Demo.WcfService
{
    [DataContract]
    public class Status
    {
        string desc = "Success";
        bool success = true;

        [DataMember]
        public string Description
        {
            get { return desc; }
            set { desc = value; }
        }

        [DataMember]
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }
    }
}