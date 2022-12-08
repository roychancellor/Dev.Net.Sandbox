using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiWithJSONAndXML.Models
{
    // This is a class just to model what the IHttpActionResult returns for Bad Request or Internal Server Error
    // so the Swagger page will show them for reference
    public class Error
    {
        public string Message { get; set; }
    }
}
