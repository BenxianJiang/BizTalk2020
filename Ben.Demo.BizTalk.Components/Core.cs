using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ben.Demo.BizTalk.Components
{
    public class Core
    {
        public static void GetBenLogger(string instanceId, 
                                        string callerName,
                                        out log4net.Ext.Serializable.SLog logger, 
                                        out log4net.helpers.PropertiesCollectionEx logProgs)
        {
            logger = log4net.Ext.Serializable.SLogManager.GetLogger(@"Ben.Demo.BizTalk", callerName);
            logger.RegistryConfigurator();
            logProgs = new log4net.helpers.PropertiesCollectionEx();
            logProgs.Set("InstanceId", instanceId);
        }

        /// <summary>
        /// Comments
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="surName"></param>
        /// <returns></returns>
        public static string SayWelcome(string firstName, string surName)
        {
            return string.Format("Hello, {0} {1}, Welcome!", firstName, surName);
        }

        /// <summary>
        /// Comments again
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="fromNamespace"></param>
        /// <param name="toNamespace"></param>
        /// <returns></returns>
        public static XmlDocument UpdateMessageNamespace(XmlDocument msg, string fromNamespace, string toNamespace)
        {
            string msgXmlString = msg.OuterXml;
            string result = msgXmlString.Replace(fromNamespace, toNamespace);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(result);

            return xml;
        }

        public static XmlDocument GetHelloWorldRequest(string yourName)
        {
            XmlDocument result = new XmlDocument();
            string xmlString = @"<ns0:HelloWorld xmlns:ns0='http://ben.demo.wcf/helloservice'>
                                    <ns0:yourName>{0}</ns0:yourName>
                                </ns0:HelloWorld> ";

            xmlString = string.Format(xmlString, yourName);
            result.LoadXml(xmlString);

            return result;
        }
    }
}
