using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROBESimulator.Common.Contracts
{
    public interface IPROBEData
    {
        bool Verbose { get; set; }
        string IBCID { get; set; }
        DateTime TimeStamp { get; set; }
    }
}
