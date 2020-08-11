using LazyWeChat.Utility;
using System;

namespace LazyWeChat.Models
{
    public enum APIType
    {
        OfficialAccount,
        MiniProgram,
        QY,
    }

    public class LazyWeChatConfiguration
    {
        public string AppID { get; set; }

        public string AppSecret { get; set; }

        public string EncodingAESKey { get; set; }

        public string APIType { get; set; }

        public APIType Type { get => (APIType)Enum.Parse(typeof(APIType), APIType); }

        public string Token { get; set; }

        public string MCHID { get; set; }

        public string Key { get; set; }

        public string NonceStr { get => Guid.NewGuid().ToString().Replace("-", ""); }

        public string Timestamp { get => UtilRepository.GetUTCTicks().ToString(); }

        public string SignType { get => "MD5"; }

        public string LazyWechatListener { get => "LazyWechatListener"; }

        public string NativeNotifyListener { get => "NativeNotifyListener"; }

    }
}
