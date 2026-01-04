using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Common.Config
{
    public interface IConfigProvider
    {
        void Refresh();
    }
}
