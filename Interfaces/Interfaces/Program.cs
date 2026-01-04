using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /* 1. In the Interfaces folder, create an interface named IDataGetter with a single parameterless method named Get that returns a string.
             * 2. In the Implementations folder, create a class called MemoryDataGetter that implements IDataGetter and whose Get method simply returns a constant string.
             * 3. Same as 1, but called ConsoleDataGetter where the Get method reads a string from the user via the console and returns the string.
             * 4. Same as 1 and 2, but called FileDataGetter where the Get method reads all lines from a text file into a single string and returns the string.
             * 5. In this class (Program), create a public static method called GetData that takes in a single IDataGetter parameter, calls its Get method, and returns the string.
             *    (If the passed in parameter is null, return null.)
             * 
             * Within this Main method:
             * 1. Create three IDataGetter variables, one for each concrete implementation above.
             * 2. Call the GetData method three times, one for each IDataGetter variable, and write each result to the console.
             * 3. Build and run the program.
             */
        }
    }
}
