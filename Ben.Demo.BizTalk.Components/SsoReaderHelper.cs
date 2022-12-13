using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ben.Demo.BizTalk.Components
{
    public static class SsoReaderHelper
    {
        public static string GetSsoValue(string key)
        { 
            string result = SSOSettingsFileReader.ReadString(Constants.AffiliateApplicationName, key);

            return result;
        }
    }
}