using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;


namespace Ben.Demo.WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constant.WcfNameSpace)]
    public class UploadService : IUploadService
    {
        List<Document> IUploadService.UploadDocuments(List<Document> documents)
        {
            List<Document> docs = new List<Document>();
            //store documents into file

            foreach(var doc in documents)
            {
                string fileName = Guid.NewGuid().ToString() + ".pdf";
                string filePath = Path.Combine(Constant.WcfDocStore, fileName);

                //write to path;
                File.WriteAllBytes(filePath, doc.Content);
                Document docResp = new Document();
                docResp.FileName = fileName;

                docs.Add(docResp);
            }

            return docs;
        }
    }
}
