using Models.Query;
using Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MultiStateQueryIOInterface
{
    [ServiceContract(Namespace ="")]
    public interface IMultiStateResponseMocks
    {
        [OperationContract]
        string Process(string XMLQueryBody);
        
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare)]
        DR ProcessDQ(DQ toProcess);
    }
}
