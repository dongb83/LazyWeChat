using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LazyWeChat.Models.OfficialAccount
{
    public class WeChatKFMessage
    {
        protected string type;

        public string touser { get; set; }

        public string msgtype { get => type; }
    }

    public class WeChatKFTextMessage : WeChatKFMessage
    {
        private TextMessage message;
        private CustomServiceMessage customeservice;

        public WeChatKFTextMessage()
        {
            type = "text";
            message = new TextMessage();
            customeservice = new CustomServiceMessage();
        }

        public TextMessage text { get => message; set => message = value; }

        public CustomServiceMessage customservice { get => customeservice; set => customeservice = value; }

        public class TextMessage
        {
            public string content { get; set; }
        }

        public class CustomServiceMessage
        {
            public string kf_account { get; set; }
        }
    }

    public class WeChatKFImageMessage : WeChatKFMessage
    {
        private ImageMessage message;
        public WeChatKFImageMessage()
        {
            type = "image";
            message = new ImageMessage();
        }

        /// <summary>
        /// test
        /// </summary>
        public ImageMessage image { get => message; set => message = value; }

        public class ImageMessage
        {
            public string media_id { get; set; }
        }
    }

    public class WeChatKFVoiceMessage : WeChatKFMessage
    {
        private VoiceMessage message;
        public WeChatKFVoiceMessage()
        {
            type = "voice";
            message = new VoiceMessage();
        }

        public VoiceMessage voice { get => message; set => message = value; }

        public class VoiceMessage
        {
            public string media_id { get; set; }
        }
    }

    public class WeChatKFVideoMessage : WeChatKFMessage
    {
        private VideoMessage message;
        public WeChatKFVideoMessage()
        {
            type = "video";
            message = new VideoMessage();
        }

        public VideoMessage video { get => message; set => message = value; }

        public class VideoMessage
        {
            public string media_id { get; set; }

            public string thumb_media_id { get; set; }

            public string title { get; set; }

            public string description { get; set; }
        }
    }

    public class WeChatKFMusicMessage : WeChatKFMessage
    {
        private MusicMessage message;
        public WeChatKFMusicMessage()
        {
            type = "music";
            message = new MusicMessage();
        }

        public MusicMessage music { get => message; set => message = value; }

        public class MusicMessage
        {
            public string title { get; set; }

            public string description { get; set; }

            public string musicurl { get; set; }

            public string hqmusicurl { get; set; }

            public string thumb_media_id { get; set; }
        }
    }

    public class WeChatKFExternalNewsMessage : WeChatKFMessage
    {
        private NewsMessage message;
        public WeChatKFExternalNewsMessage()
        {
            type = "news";
            message = new NewsMessage();
        }

        public NewsMessage news { get => message; set => message = value; }

        public class NewsMessage
        {
            private List<ArticleMessage> _msg;
            public NewsMessage()
            {
                _msg = new List<ArticleMessage>();
            }

            public List<ArticleMessage> articles { get => _msg; set => _msg = value; }
        }

        public class ArticleMessage
        {
            public string title { get; set; }

            public string description { get; set; }

            public string url { get; set; }

            public string picurl { get; set; }
        }
    }

    public class WeChatKFNewsMessage : WeChatKFMessage
    {
        private MPNewsMessage message;
        public WeChatKFNewsMessage()
        {
            type = "mpnews";
            message = new MPNewsMessage();
        }

        public MPNewsMessage mpnews { get => message; set => message = value; }

        public class MPNewsMessage
        {
            public string media_id { get; set; }
        }
    }

    public class WeChatKFMenuMessage : WeChatKFMessage
    {
        private MenuMessage message;
        public WeChatKFMenuMessage()
        {
            type = "msgmenu";
            message = new MenuMessage();
        }

        public MenuMessage msgmenu { get => message; set => message = value; }

        public class MenuMessage
        {
            private List<ContentMessage> _msg;
            public MenuMessage()
            {
                _msg = new List<ContentMessage>();
            }

            public string head_content { get; set; }

            public string tail_content { get; set; }

            public List<ContentMessage> list { get => _msg; set => _msg = value; }
        }

        public class ContentMessage
        {
            public string id { get; set; }

            public string content { get; set; }
        }
    }

    public class WeChatKFCardMessage : WeChatKFMessage
    {
        private CardMessage message;
        public WeChatKFCardMessage()
        {
            type = "wxcard";
            message = new CardMessage();
        }

        public CardMessage wxcard { get => message; set => message = value; }

        public class CardMessage
        {
            public string card_id { get; set; }
        }
    }

    public class WeChatKFMiniMessage : WeChatKFMessage
    {
        private MiniMessage message;
        public WeChatKFMiniMessage()
        {
            type = "miniprogrampage";
            message = new MiniMessage();
        }

        public MiniMessage miniprogrampage { get => message; set => message = value; }

        public class MiniMessage
        {
            public string title { get; set; }

            public string appid { get; set; }

            public string pagepath { get; set; }

            public string thumb_media_id { get; set; }
        }
    }
}
