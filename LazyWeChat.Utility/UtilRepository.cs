using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace LazyWeChat.Utility
{
    public static class UtilRepository
    {
        static readonly string IMPLEMENTATIONASSEMBLYNAME = "LazyWeChat.Implementation";

        public static bool IsPropertyExist(dynamic data, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data is ExpandoObject)
                return ((IDictionary<string, object>)data).ContainsKey(propertyName);

            return data.GetType().GetProperty(propertyName) != null;
        }

        public static dynamic ParseAPIResult(string json)
        {
            IDictionary<string, object> keyValuePairs = new Dictionary<string, object>();

            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                if (json.StartsWith("{{"))
                    return json;

                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                foreach (var item in dict)
                {
                    var obj = ParseAPIResult(item.Value.ToString());
                    keyValuePairs.Add(item.Key, obj);
                }

                return keyValuePairs.ToDynamic();
            }
            else if (json.StartsWith("[") && json.EndsWith("]"))
            {
                var list = new List<object>();
                var obj = JsonConvert.DeserializeObject<List<object>>(json);

                if (obj != null)
                {
                    obj.ForEach(i =>
                    {
                        if (!string.IsNullOrEmpty(i.ToString()))
                        {
                            var res = ParseAPIResult(i.ToString());
                            list.Add(res);
                        }
                    });
                }

                return list;
            }
            else
            {
                return json;
            }
        }

        public static string GenerateSignature(string noncestr, string timestamp, string url, string ticket, out string outString)
        {
            SortedDictionary<string, string> sortedDict = new SortedDictionary<string, string>();
            sortedDict.Add("noncestr", noncestr);
            sortedDict.Add("timestamp", timestamp);
            sortedDict.Add("url", url);
            sortedDict.Add("jsapi_ticket", ticket);

            StringBuilder sb = new StringBuilder();
            foreach (var pair in sortedDict)
            {
                var str = string.Format("{0}={1}&", pair.Key, pair.Value);
                sb.Append(str);
            }
            var combinedStr = sb.ToString();
            combinedStr = combinedStr.Substring(0, combinedStr.Length - 1);
            outString = combinedStr;
            var signature = SHA1(combinedStr);
            return signature.ToLower();

            string SHA1(string text)
            {
                byte[] cleanBytes = Encoding.Default.GetBytes(text);
                byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }

        public static Type GetImplementation(string implementationClassName)
        {
            Assembly assembly = Assembly.Load(IMPLEMENTATIONASSEMBLYNAME);
            var typies = assembly.GetTypes();

            for (int i = 0; i < typies.Length; i++)
            {
                if (implementationClassName == typies[i].Name)
                {
                    var type = typies[i];
                    return type;
                }
            }

            throw new ArgumentNullException(implementationClassName);
        }

        public static long GetUTCTicks()
        {
            return Convert.ToInt64((DateTime.Now - DateTime.Parse("1970-1-1")).TotalMilliseconds);
        }
    }
}
