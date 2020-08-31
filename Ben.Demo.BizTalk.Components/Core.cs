using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ben.Demo.BizTalk.Components
{
    public class Core
    {
        public static string SayWelcome(string firstName, string surName)
        {
            return string.Format("Hello, {0} {1}, Welcome!", firstName, surName);
        }
    }
}
