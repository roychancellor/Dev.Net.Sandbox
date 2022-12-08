using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MultiStateQueryPOC.Utilities
{
    public class Utilities
    {
        public static string ListToString<T>(List<T> toConvert)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var x in toConvert)
            {
                sb.AppendLine(x.ToString());
            }
            return sb.ToString();
        }

        public static string QueryToResponseMessageKey(string queryMKE)
        {
            return queryMKE.Replace("Q", "R");
        }
    }
}