using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Ben.Demo.WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = Constant.WcfNameSpace)]
    public interface IUploadService
    {
        [OperationContract]
        List<Document> UploadDocuments(List<Document> documents);
    } 
}
