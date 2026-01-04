using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PROBESimulator.IO.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerProducesAttribute : Attribute
    {
        public SwaggerProducesAttribute(params string[] contentTypes)
        {
            ContentTypes = contentTypes;
        }

        public IEnumerable<string> ContentTypes { get; }
    }
}