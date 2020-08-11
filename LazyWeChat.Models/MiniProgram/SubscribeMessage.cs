using LazyWeChat.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace LazyWeChat.Models.MiniProgram
{
    public enum MiniprogramState
    {
        developer,
        trial,
        formal,
    }

    public enum Language
    {
        zh_CN,
        en_US,
        zh_HK,
        zh_TW,
    }

    public class SubscribeMessage
    {
        public SubscribeMessage()
        {
            rawData = new Dictionary<string, string>();
        }

        public string touser { get; set; }

        public string template_id { get; set; }

        public string page { get; set; }

        public string miniprogram_state { get; set; }

        public string lang { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> rawData { get; set; }

        public dynamic data
        {
            get
            {
                SortedDictionary<string, dynamic> dict = new SortedDictionary<string, dynamic>();
                foreach (var item in rawData)
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
    }
}
