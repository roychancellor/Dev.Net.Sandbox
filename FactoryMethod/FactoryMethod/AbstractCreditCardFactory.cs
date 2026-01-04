using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    public abstract class AbstractCreditCardFactory
    {
        protected abstract ICreditCard MakeProduct(string productType);

        public ICreditCard Create(string productType)
        {
            ICreditCard creditCard = this.MakeProduct(productType);
            return creditCard;
        }
    }
}
