using System;
using System.IO;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System.ComponentModel;
using System.Resources;
using System.Text;
using System.Xml;

namespace Ben.Demo.BizTalk.Components
{
	/// <summary>
	/// Disassembler Component to split incoming message - comments
	/// </summary>
	[ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
	[ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
	[System.Runtime.InteropServices.Guid("02349D44-FF36-451D-8C04-E3AEC48230D9")]
	public class SplitMessage : IBaseComponent,
		IDisassemblerComponent,
		IComponentUI,
		IPersistPropertyBag
	{

		System.Collections.Queue qOutputMsgs = new System.Collections.Queue();

		public SplitMessage()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string Description
		{
			get
			{
				return "Ben Demo Disassembler Component to split incoming message";
			}
		}

		public string Name
		{
			get
			{
				return "BenDemo SplitMessage Disassembler";
			}
		}

		public string Version
		{
			get
			{
				return "1.0.0.0";
			}
		}

		public System.Collections.IEnumerator Validate(object projectSystem)
		{
			return null;
		}

		public System.IntPtr Icon
		{
			get
			{
				return new System.IntPtr();
			}
		}

		public void GetClassID(out Guid classID)
		{
			classID = new Guid("02349D44-FF36-451D-8C04-E3AEC48230D9");
		}

		public void InitNew()
		{
		}

		public void Load(IPropertyBag propertyBag, int errorLog)
		{
		}

		public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
		{
		}

		public void Disassemble(IPipelineContext pContext, IBaseMessage pInMsg)
		{
            Stream inSteam;
			XmlDocument xml;
			IBaseMessagePart msgPart;

			try
			{
				msgPart = pInMsg.BodyPart;
				inSteam = msgPart.GetOriginalDataStream();
				XmlReader xReader = XmlReader.Create(inSteam);
				//inSteam.Seek(0, SeekOrigin.Begin);
				//byte[] bufferBytes = new byte[inSteam.Length];
				//inSteam.Read(bufferBytes, 0, Convert.ToInt32(inSteam.Length));
								
				//string xmlString = Encoding.UTF8.GetString(bufferBytes);
				

				xml = new XmlDocument();
				//string inFile = @"C:\Ben\Ben.Demo.BizTalk\Article_Sample.xml";
				xml.Load(xReader);

				File.AppendAllText(@"C:\Temp\splitLog.txt", "incoming msg = " + xml.OuterXml + Environment.NewLine);

				string xPath = "/*[local-name()='Articles' and namespace-uri()='http://Ben.Demo.BizTalk.Schemas.ArticleSchema']/*[local-name()='Article' and namespace-uri()='']";
				XmlNodeList articles = xml.SelectNodes(xPath);
				string head = @"<ns0:Articles xmlns:ns0='http://Ben.Demo.BizTalk.Schemas.ArticleSchema'>";
				string tail = @"</ns0:Articles>";

				//put into message queue for each article
				IBaseMessage outMsg;
				foreach (XmlNode art in articles)
				{
					string oneArticle = head + art.OuterXml + tail;
					File.AppendAllText(@"C:\Temp\splitLog.txt", oneArticle + Environment.NewLine);

					byte[] artBytes = Encoding.UTF8.GetBytes(oneArticle);
					MemoryStream strmMem = new MemoryStream(artBytes);
					strmMem.Seek(0, SeekOrigin.Begin);

					outMsg = pContext.GetMessageFactory().CreateMessage();
					outMsg.AddPart("Body", pContext.GetMessageFactory().CreateMessagePart(), true);
					outMsg.BodyPart.Data = strmMem;
					//outMsg.Context = PipelineUtil.CloneMessageContext(pInMsg.Context);
					outMsg.Context = pInMsg.Context;

					qOutputMsgs.Enqueue(outMsg);
				}
			}
			catch (Exception ex)
			{
				File.AppendAllText(@"C:\Temp\splitLog.txt", ex.Message + Environment.NewLine);
			}
		}

        public IBaseMessage GetNext(IPipelineContext pContext)
        {
			IBaseMessage outMsg = null;

			if (qOutputMsgs.Count > 0)
			{
				outMsg = (IBaseMessage)qOutputMsgs.Dequeue();
				outMsg.BodyPart.Data.Seek(0, SeekOrigin.Begin);
			}
			
			return outMsg;
        }
    }
}