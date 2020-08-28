using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace LazyWeChat.Utility
{
    public static class ObjectExtensions
    {
        public static dynamic ToDynamic(this object obj)
        {
            var dict = obj as IDictionary<string, object>;

            dynamic result = new ExpandoObject();

            foreach (var entry in dict)
            {
                (result as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(entry.Key, entry.Value));
            }

            return result;
        }

        public static string ToAbsoluteUri(this HttpRequest httpRequest)
        {
            var x_http_scheme = httpRequest.Headers["X-Http-Scheme"].ToString();
            var scheme = string.IsNullOrEmpty(x_http_scheme) ? httpRequest.Scheme : x_http_scheme;
            return new StringBuilder()
                .Append(scheme)
                .Append("://")
                .Append(httpRequest.Host)
                .Append(httpRequest.PathBase)
                .Append(httpRequest.Path)
                .Append(httpRequest.QueryString)
                .ToString();
        }

        public static SortedDictionary<string, object> FromXml(this string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException(nameof(xml));

            SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                if (!xe.InnerXml.Replace("<!", "").Contains("<"))
                    m_values[xe.Name] = xe.InnerText;
                else
                {
                    m_values[xe.Name] = FromXml($"<xml>{xe.InnerXml}</xml>");
                }
            }

            return m_values;
        }

        public static string SHA1(this string text)
        {
            byte[] cleanBytes = Encoding.Default.GetBytes(text);
            byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }

        public static string MD5(this string encypStr)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            byte[] inputBye;
            byte[] outputBye;

            inputBye = Encoding.Default.GetBytes(encypStr);
            outputBye = m5.ComputeHash(inputBye);

            retStr = BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }

    }
}
