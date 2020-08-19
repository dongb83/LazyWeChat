using LazyWeChat.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;

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

        /// <summary>
        /// 接受者openid
        /// </summary>
        public string touser { get; set; }

        /// <summary>
        /// 所需下发的订阅模板id
        /// </summary>
        public string template_id { get; set; }

        /// <summary>
        /// 点击模板卡片后的跳转页面
        /// </summary>
        public string page { get; set; }

        /// <summary>
        /// 跳转小程序类型:developer为开发版,trial为体验版,formal为正式版,默认为正式版
        /// </summary>
        public string miniprogram_state { get; set; }

        /// <summary>
        /// 进入小程序查看”的语言类型
        /// </summary>
        public string lang { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string> rawData { get; set; }

        /// <summary>
        /// 模板内容的dynamic格式,用于序列化
        /// </summary>
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
