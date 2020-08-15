using LazyWeChat.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;

namespace LazyWeChat.Models
{
    public class UniformMessage
    {
        public string touser { get; set; }
    }

    public class WeAppUniformMessage : UniformMessage
    {
        public WeAppUniformMessage()
        {
            mp_template_msg = new mp_template();
            mp_template_msg.miniprogram = new miniprogram();
            mp_template_msg.rawData = new List<(string, string, string)>();

            weapp_template_msg = new weapp_template();
            weapp_template_msg.rawData = new Dictionary<string, string>();
        }

        public weapp_template weapp_template_msg { get; set; }

        public mp_template mp_template_msg { get; set; }

        public class weapp_template
        {
            public string template_id { get; set; }

            public string page { get; set; }

            public string form_id { get; set; }

            [JsonIgnore]
            public Dictionary<string, string> rawData { get; set; }

            public dynamic data
            {
                get
                {
                    SortedDictionary<string, dynamic> dict = new SortedDictionary<string, dynamic>();
                    foreach(var item in rawData)
                    {
                        dynamic obj = new ExpandoObject();
                        obj.value = item.Value;
                        dict.Add(item.Key, obj);
                    }
                    var json = JsonConvert.SerializeObject(dict);
                    var _data = UtilRepository.ParseAPIResult(json);
                    return _data;
                }
            }

            public string emphasis_keyword { get; set; }
        }
    }

    public class MPUniformMessage : UniformMessage
    {
        public MPUniformMessage()
        {
            mp_template_msg = new mp_template();
            mp_template_msg.miniprogram = new miniprogram();
            mp_template_msg.rawData = new List<(string, string, string)>();
        }

        public mp_template mp_template_msg { get; set; }
    }

    public class mp_template
    {
        public string appid { get; set; }

        public string url { get; set; }

        public miniprogram miniprogram { get; set; }

        [JsonIgnore]
        public List<(string, string, string)> rawData { get; set; }

        public dynamic data
        {
            get
            {
                SortedDictionary<string, dynamic> dict = new SortedDictionary<string, dynamic>();
                for (int i = 0; i < rawData.Count; i++)
                {
                    dynamic item = new ExpandoObject();
                    item.value = rawData[i].Item2;
                    if (!string.IsNullOrEmpty(rawData[i].Item3))
                        item.color = rawData[i].Item3;
                    dict.Add(rawData[i].Item1, item);
                }
                var json = JsonConvert.SerializeObject(dict);
                var _data = UtilRepository.ParseAPIResult(json);
                return _data;
            }
        }

        public string template_id { get; set; }
    }

    public class miniprogram
    {
        public string appid { get; set; }

        public string pagepath { get; set; }
    }
}
