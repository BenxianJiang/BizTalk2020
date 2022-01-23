using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;
using Microsoft.XLANGs.BaseTypes;
using System.Reflection;
using System.Threading;

namespace Ben.Demo.BizTalk.Components
{  
    public static class XmlHelper
    {
        private static string targetNamespace = "http://www.w3.org/2001/XMLSchema";

        /// <summary>
        /// Use LinQ object to change root name.
        /// Change root node from "RequestProvider" to "Collection"
        /// </summary>
        /// <param name="request">XmlDocument with root name "RequestProvider"</param>
        /// <returns>XmlDocument with root name "Collection"</returns>
        public static XmlDocument ChangeProviderRootNameLinq(XmlDocument request)
        {
            XNamespace ns = Constants.ProviderNamespace;
            XDocument xDoc = ToXDocument(request);
            XElement root = xDoc.Root;
            root.Name = ns + "Collection";

            //convert to XmlDocument
            XmlDocument result = ToXmlDocument(xDoc);

            return result;         
        }

        /// <summary>
        /// Use XmlDocument to replace the root name.
        /// </summary>
        /// <param name="requestProvider">XmlDocument with root name "RequestProvider"</param>
        /// <returns>XmlDocument with root name "Collection"</returns>
        public static XmlDocument ChangeProviderRootNameXml(XmlDocument requestProvider)
        {
            string xmlString = requestProvider.OuterXml;

            //replace root node name from "ProviderRequest" to "Collection"
            string newXmlString = xmlString.Replace("ProviderRequest", "Collection");

            //convert to XmlDocument
            XmlDocument result = new XmlDocument();
            result.LoadXml(newXmlString);

            return result;
        }

        /// <summary>
        /// This method is to handle if element is missing
        /// </summary>
        /// <param name="element"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string SafeElementValue(XElement element, string defaultValue = "")
        {
            if (element != null)
                return element.Value;

            return defaultValue;
        }

        /// <summary>
        /// This method is to handle if attribute is missing
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string SafeAttributeValue(XAttribute attribute, string defaultValue = "")
        {
            if (attribute != null)
                return attribute.Value;

            return defaultValue;
        }

        /// <summary>
        /// Convert an XElement to an XmlDocument.
        /// </summary>
        /// <param name="xelement"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(XElement xelement)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(xelement.CreateReader());
            return xmldoc;
        }

        /// <summary>
        /// Converts an XDocument to an XmlDocument.
        /// </summary>
        /// <param name="xdoc">The XDocument to convert.</param>
        /// <returns>The equivalent XmlDocument.</returns>
        public static XmlDocument ToXmlDocument(XDocument xdoc)
        {
            var xmldoc = new XmlDocument();
            xmldoc.Load(xdoc.CreateReader());
            return xmldoc;
        }

        /// <summary>
        /// Converts an XmlDocument to an XDocument.
        /// </summary>
        /// <param name="xmldoc">The XmlDocument to convert.</param>
        /// <returns>The equivalent XDocument.</returns>
        public static XDocument ToXDocument(XmlDocument xmldoc)
        {
            return XDocument.Load(xmldoc.CreateNavigator().ReadSubtree());
        }

        /// <summary>
        /// Converts an XmlDocument to an XElement.
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public static XElement ToXElement(XmlDocument xmldoc)
        {
            return ToXDocument(xmldoc).Root;
        }

        /// <summary>
        /// Converts an XElement to an XmlElement.
        /// </summary>
        /// <param name="xelement">The XElement to convert.</param>
        /// <returns>The equivalent XmlElement.</returns>
        public static XmlElement ToXmlElement(XElement xelement)
        {
            return new XmlDocument().ReadNode(xelement.CreateReader()) as XmlElement;
        }

        /// <summary>
        /// Converts an XmlElement to an XElement.
        /// </summary>
        /// <param name="xmlelement">The XmlElement to convert.</param>
        /// <returns>The equivalent XElement.</returns>
        public static XElement ToXElement(XmlElement xmlelement)
        {
            return XElement.Load(xmlelement.CreateNavigator().ReadSubtree());
        }

        //Creates an object from an XML string.
        public static object FromXmlString(string Xml, System.Type objType)
        {

            XmlSerializer ser;
            ser = new XmlSerializer(objType);
            StringReader stringReader;
            stringReader = new StringReader(Xml);
            XmlTextReader xmlReader;
            xmlReader = new XmlTextReader(stringReader);
            object obj;
            obj = ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return obj;
        }

        public static object FromXmlFile(string xmlFileName, System.Type objType)
        {
            string xml = File.ReadAllText(xmlFileName);
            return FromXmlString(xml, objType);
        }

        //Serializes the <i>Obj</i> to an XML string.
        public static string ToXml(object obj, System.Type objType)
        {
            XmlSerializer ser;
            ser = new XmlSerializer(objType, targetNamespace);
            MemoryStream memStream;
            memStream = new MemoryStream();
            XmlTextWriter xmlWriter;
            xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            xmlWriter.Namespaces = true;
            ser.Serialize(xmlWriter, obj, GetNamespaces());
            xmlWriter.Close();
            memStream.Close();
            string xml;
            xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return xml;
        }

        public static string AddPackageNamespace(string xmlString)
        {
            string result = string.Empty;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlString);
            string packagePath = @"/*[local-name()='Collection' and namespace-uri()='urn:dxc.esb.aibp.provider/v1']/*[local-name()='Article' and namespace-uri()='urn:dxc.esb.aibp.provider/v1']/*[local-name()='Package' and namespace-uri()='urn:dxc.esb.aibp.provider/v1']";
            string lixiNameSpace = @"urn:dxc.esb.aibp.lixi.cal/v1";

            XmlNode node = xml.SelectSingleNode(packagePath);
            XmlAttribute nsAttribute = xml.CreateAttribute("xmlns");
            nsAttribute.Value = lixiNameSpace;
            node.Attributes.Append(nsAttribute);

            result = xml.OuterXml;

            return result;
        }

        //This will returns the set of included namespaces for the serializer.
        public static XmlSerializerNamespaces GetNamespaces()
        {
            XmlSerializerNamespaces ns;
            ns = new XmlSerializerNamespaces();

            //ns.Add(string.Empty, "urn:dxc.esb.aibp.provider/v1");
            ns.Add("xs", "http://www.w3.org/2001/XMLSchema");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            
            return ns;
        }

        #region Validation Xml use for Orchestration

        private static StringBuilder _sb = new StringBuilder(string.Empty);

        /// <summary>
        /// Validate XML for its Type name (root node name normally).
        /// </summary>
        /// <param name="xml">XmlDocument object, ProviderRequest xml.</param>
        /// <param name="typeName">string, The type name of XML object. eg. "ProviderRequest"</param>
        /// <returns>String the validation message if failed else empty string.</returns>
        public static string Validate(XmlDocument xml, string typeName, log4net.Ext.Serializable.SLog logger, log4net.helpers.PropertiesCollectionEx loggerProp)
        {
            _sb = new StringBuilder(string.Empty);

            try
            {
                //Get Schema Assembly
                Assembly ass = GetSchemaAssembly();

                //get the type only match the typeName, eg. "ProviderRequest"
                Type t = ass.GetTypes().FirstOrDefault(x => x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

                //Add the found schema into XmlSchemaSet
                if (t != null)
                {
                    XmlSchema s = (Activator.CreateInstance(t) as SchemaBase).Schema;
                    xml.Schemas.Add(s);
                }

                //Type[] ts = ass.GetTypes();
                //foreach (Type t in ts)
                //{
                //    if (t.Name == "ProviderRequest")
                //    {
                //        XmlSchema s = (Activator.CreateInstance(t) as SchemaBase).Schema;
                //        xmlSet.Add(s);
                //        xml.Schemas.Add(s);
                //    }

                //    //string fn = t.FullName;
                //}

                xml.Validate(new ValidationEventHandler(ValidationHandler));
            }
            catch (Exception ex)
            {
                logger.Error(loggerProp, "Error XmlHelper.Validate " + ex.ToString());
                throw ex;
            }

            string result = _sb.ToString().Trim();
            
            return result;
        }

        /// <summary>
        /// Store any validation error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void ValidationHandler(object sender, ValidationEventArgs args)
        {
            string msg = string.Format("Line: {0}, Position: {1}, Error: {2}", args.Exception.LineNumber, args.Exception.LinePosition, args.Message);
            _sb.AppendLine(msg);
        }

        /// <summary>
        /// Get Schema Assembly object by either fully qualified name or
        /// loading from schema dll from GAC.
        /// </summary>
        /// <returns>Assembly object</returns>
        public static Assembly GetSchemaAssembly()
        {
            //Fully Qualified Name - by using string fqn = AssemblyName.GetAssemblyName(assemblyFile).ToString();
            string fqn = Constants.SchemaFullyQualifiedName;
            string assemblyFile = @"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\ES.FS.WG.MSAPI.Schemas\v4.0_1.0.0.0__1637b744db65b335\ES.FS.WG.MSAPI.Schemas.dll";
            
            //from FQN first
            Assembly ass = Assembly.Load(fqn);

            if (ass == null)
            {
                //load from schema dll file in GAC
                ass = Assembly.LoadFile(assemblyFile);
            }

            return ass;
        }

        #endregion
    }
}
