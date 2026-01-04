using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    internal class CreditCardFactory : AbstractCreditCardFactory
    {
        protected override ICreditCard MakeProduct(string productType)
        {
            // The Activator method of creating the object is inspired by:
            // https://visualstudiomagazine.com/articles/2011/01/27/the-factory-pattern-in-net-part-3.aspx
            var cardType = Type.GetType($"FactoryMethod.{productType}Card");
            ICreditCard toReturn = Activator.CreateInstance(cardType) as ICreditCard;
            return toReturn;
        }
    }
}
