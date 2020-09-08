using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace Ben.Demo.UnitTest
{
    [TestClass]
    public class BizArticleUploadDownloadTest
    {
        [TestMethod]
        public void UploadTestMethod()
        {
            string filePath = @"C:\GitHub\Ben\Ben.Demo.BizTalk\Ben.Demo.UnitTest\Data\UploadFile.pdf";

            BizArticleService.BizArticleServiceClient client = new BizArticleService.BizArticleServiceClient("CustomBinding_ITwoWayAsync");

            List<BizArticleService.BizArticlesArticle> arts = new List<BizArticleService.BizArticlesArticle>();
            BizArticleService.BizArticlesArticle art = new BizArticleService.BizArticlesArticle();
            art.ArticleName = "article 1";
            art.ArticleContent = File.ReadAllBytes(filePath);
            arts.Add(art);

            art.ArticleName = "Article 2";
            art.ArticleContent = File.ReadAllBytes(filePath);
            arts.Add(art);

            BizArticleService.BizArticlesArticle[] req = arts.ToArray();

            //for windows authentication
            client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
            //or use client.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential("userName", "password", "domain");

            client.Upload(ref req);

            foreach(var r  in req)
            {
                var name = r.ArticleName;
                var content = r.ArticleContent;
            }
        }
    }
}
