using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WixServiceWinApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {


                //Comment:
                //use http://vboxwin2019/BiztalkWIXServices/V1/WIXService.svc?wsdl to generate proxy;
                //Note - Dot NOT use singleWSDL!
                //also manually change CanonicalType to AIBPCanonical in Reference.cs
                //Update WixServiceClient object also in 

                WcfWixService.WIXServiceClient client = new WcfWixService.WIXServiceClient("CustomBinding_ITwoWayAsyncVoid");

                //TODO: populate Request object.
                //WcfWixService.AIBPCanonical req = new WcfWixService.AIBPCanonical();

                //use request file:
                string xmlFile = Path.Combine(Environment.CurrentDirectory, @"InlineAttachmentFilePath.xml");
                WcfWixService.AIBPCanonical req = (WcfWixService.AIBPCanonical)Core.DeserializeFromXmlFile<WcfWixService.AIBPCanonical>(xmlFile);

                //for windows authentication
                client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;

                //client.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential("dv-svc_biztalk01", "Welcome1", "edscs-npe");

                client.UploadDocument(ref req);

                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
