using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Ben.Demo.WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDownloadService" in both code and config file together.
    [ServiceContract(Namespace = Constant.WcfNameSpace)]
    public interface IDownloadService
    {
        [OperationContract]
        List<Document> DownloadDocuments(List<Document> request);
    }
}
