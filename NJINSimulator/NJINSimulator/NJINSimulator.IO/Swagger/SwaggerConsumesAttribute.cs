using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NJINSimulator.IO.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerConsumesAttribute : Attribute
    {
        public SwaggerConsumesAttribute(params string[] contentTypes)
        {
            ContentTypes = contentTypes;
        }

        public IEnumerable<string> ContentTypes { get; }
    }
}