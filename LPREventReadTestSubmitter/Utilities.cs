using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LPREventReadTestSubmitter
{
    public class Utilities
    {
        public static TestConfiguration GetConfigFromArgs(string[] args)
        {
            TestConfiguration toReturn = new TestConfiguration();
            return toReturn;
        }

        public static string Serialize<T>(T toSerialize)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));

            using(var writer = new StringWriter())
            {
                try
                {
                    xs.Serialize(writer, toSerialize);
                }
                catch (Exception)
                {
                    return string.Empty;
                }

                return writer.ToString();
            }
        }

        public static async Task<HttpResponseMessage> PostToAPI(string toPost)
        {
            using(var client = new HttpClient())
            {
                var content = new StringContent(toPost, Encoding.UTF8, "application/xml");
                var response = await client.PostAsync("https://reqbin.com/echo/post/xml", content);
                return response;
            }
        }
    }
}
