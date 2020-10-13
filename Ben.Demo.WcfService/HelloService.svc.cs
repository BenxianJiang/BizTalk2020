using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Ben.Demo.WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HelloService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select HelloService.svc or HelloService.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constant.HelloNameSpace)]
    public class HelloService : IHelloService
    {
        string IHelloService.HelloWorld(string yourName)
        {
            string result = "Welcome! " + yourName;
            return result;
        }
    }
}
