using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    public class PlatinumCardFactory : AbstractCardFactory
    {
        protected override ICreditCard MakeProduct()
        {
            ICreditCard product = new PlatinumCard();
            return product;
        }
    }
}
