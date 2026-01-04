using PROBESimulator.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFOSimulator.Processors
{
    public interface IUFOProcessor
    {
        void Initialize();
        void Reset();
        int Current { get; }
    }
}
