﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ben.Demo.BizTalk.Components
{
    public class Core
    {
        public static string SayWelcome(string firstName, string surName)
        {
            return string.Format("Hello, {0} {1}, Welcome!", firstName, surName);
        }

        public static XmlDocument UpdateMessageNamespace(XmlDocument msg, string fromNamespace, string toNamespace)
        {
            string msgXmlString = msg.OuterXml;
            string result = msgXmlString.Replace(fromNamespace, toNamespace);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(result);

            return xml;
        }
    }
}
