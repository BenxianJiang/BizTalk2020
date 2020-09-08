using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Ben.Demo.UnitTest
{
    [TestClass]
    public class BizArticleUploadDownloadTest
    {
        [TestMethod]
        public void UploadTestMethod()
        {
            //make sure the behaviour of BizTalk Receive WCF service in IIS (web.config) set to True!!!
            string filePath = @"C:\GitHub\Ben\Ben.Demo.BizTalk\Ben.Demo.UnitTest\Data\UploadFile.pdf";

            BizArticleService.ArticleServiceClient client = new BizArticleService.ArticleServiceClient("CustomBinding_ITwoWayAsync");

            //List<BizArticleService.BizArticlesBizArticle> arts = new List<BizArticleService.BizArticlesBizArticle>();
            BizArticleService.BizArticlesBizArticle art = new BizArticleService.BizArticlesBizArticle();
            art.BizArticleName = "article 1";
            art.BizArticleContent = File.ReadAllBytes(filePath);
            //arts.Add(art);

            //art.BizArticleName = "Article 2";
            //art.BizArticleContent = File.ReadAllBytes(filePath);
            //arts.Add(art);

            BizArticleService.BizArticles req = new BizArticleService.BizArticles();
            req.BizArticle = art;

            //for windows authentication
            client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            //or use client.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential("userName", "password", "domain");

            //make sure the behaviour of BizTalk Receive WCF service in IIS (web.config) set to True!!!
            client.UploadDocument(ref req);

            byte[] content = req.BizArticle.BizArticleContent;
        }
    }
}
