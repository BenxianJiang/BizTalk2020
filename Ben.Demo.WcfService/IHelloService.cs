using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Ben.Demo.WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IHelloService" in both code and config file together.
    [ServiceContract(Namespace = Constant.HelloNameSpace)]
    public interface IHelloService
    {
        [OperationContract]
        string HelloWorld(string yourName);
    }
}
