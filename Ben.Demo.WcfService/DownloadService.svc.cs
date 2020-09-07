using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using MyLogger = DXC.Util.Logger;

namespace Ben.Demo.WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DownloadService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DownloadService.svc or DownloadService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constant.WcfNameSpace)]
    public class DownloadService : IDownloadService
    {
        List<Document> IDownloadService.DownloadDocuments(List<Document> request)
        {
            //Use DXC Logger
            MyLogger.Logger dLogger = new MyLogger.Logger(@"C:\Ben\Log4Net\log4net.config", "Ben.Demo", Guid.NewGuid().ToString());

            dLogger.Info("Donload begin");

            List<Document> docs = new List<Document>();

            foreach(var doc in request)
            {
                string filePath = Path.Combine(Constant.WcfDocStore, doc.FileName);
                byte[] content = File.ReadAllBytes(filePath);

                Document docResp = new Document();
                docResp.FileName = doc.FileName;
                docResp.Content = content;

                docs.Add(docResp);
            }

            dLogger.Info("Download end");

            return docs;
        }
    }
}
