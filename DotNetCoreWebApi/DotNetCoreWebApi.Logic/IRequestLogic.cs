using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreWebApi.Logic
{
    public interface IRequestLogic
    {
        Task<HandleResponse> HandleRequest();
        Task<string> ProcessRequest(HandleResponse handleResponse);
    }
}
