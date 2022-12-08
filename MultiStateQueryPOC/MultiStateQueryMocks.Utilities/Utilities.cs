using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MultiStateQueryMocks.Utilities
{
    public class Utilities
    {
        public static T Deserialize<T>(string fromString, bool throwException = false)
        {
            T toReturn = default;
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                using (var sr = new StringReader(fromString))
                {
                    toReturn = (T)xmlSerializer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw ex;
                }
            }

            return toReturn;
        }

        public static string Serialize<T>(T toSerialize, bool removeDefaultXmlNamespaces = true, bool omitXmlDeclaration = true, Encoding encoding = null, bool throwExceptions = false) where T : class
        {
            string toReturn = null;
            try
            {
                XmlSerializerNamespaces namespaces = removeDefaultXmlNamespaces ? new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }) : null;

                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = omitXmlDeclaration,
                    CheckCharacters = false,
                    Encoding = encoding ?? Encoding.UTF8
                };

                using (var stream = new StringWriter())
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, toSerialize, namespaces);
                    toReturn = stream.ToString();
                }
            }
            catch (Exception)
            {
                if (throwExceptions)
                {
                    throw; 
                }
            }

            return toReturn;
        }
    }
}
