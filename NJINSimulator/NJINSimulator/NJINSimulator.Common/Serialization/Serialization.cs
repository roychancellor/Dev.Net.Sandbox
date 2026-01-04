using NJINSimulator.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NJINSimulator.Common.Serialization
{
    public class Serialization
    {
        public static string Serialize<T>(T theObject)
        {
            //TextWriter writer;
            
            if (theObject == null)
            {
                return null;
            }

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true // need to omit XML declaration in order for the WebApi XML deserialization to work properly
                };

                StringBuilder sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb, settings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, theObject);
                }
                return sb.ToString();

                //writer = new StringWriter();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static T Deserialize<T>(string toDeser, bool rethrowException = false)
        {
            T instance = default;
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var stringreader = new StringReader(toDeser))
                {
                    instance = (T)xmlSerializer.Deserialize(stringreader);
                }
            }
            catch (Exception ex)
            {
                if (rethrowException)
                {
                    throw ex;
                }
            }

            return instance;
        }
    }
}
