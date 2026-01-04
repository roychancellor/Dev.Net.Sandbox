using PROBESimulator.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROBESimulator.Common.Contracts
{
    public class NJINTPS : IPROBEData
    {
        public bool Verbose { get; set; }
        public string IBCID { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
