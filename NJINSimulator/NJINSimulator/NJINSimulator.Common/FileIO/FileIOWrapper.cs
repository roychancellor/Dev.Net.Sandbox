using NJINSimulator.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Common.FileIO
{
    public class FileIOWrapper
    {
        public static string ReadTextFile(string fromPath)
        {
            if (fromPath.IsNullOrEmpty()) return null;

            try
            {
                return File.ReadAllText(fromPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
