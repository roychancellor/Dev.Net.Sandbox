using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROBESimulator.Common.Contracts
{
    public interface IPROBEDataDeltaT : IPROBEData
    {
        string Direction { get; set; }
    }
}
