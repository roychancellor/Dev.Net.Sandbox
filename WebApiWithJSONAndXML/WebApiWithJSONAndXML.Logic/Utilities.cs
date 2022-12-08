using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebApiWithJSONAndXML.Logic
{
    public static class Utilities
    {
        public static bool IsNullOrEmpty(this string toCheck)
        {
            return string.IsNullOrEmpty(toCheck);
        }
    }
}
