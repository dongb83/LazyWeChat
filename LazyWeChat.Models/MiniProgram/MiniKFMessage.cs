using System;
using System.Collections.Generic;
using System.Text;

namespace LazyWeChat.Models.MiniProgram
{
    public enum MiniMessageType
    {
        text,
        image,
        link,
        miniprogrampage,
    }

    public class MiniKFMessage
    {
        public string touser { get; set; }

        public string msgtype { get; set; }
    }

    public class MiniKFTextMessage : MiniKFMessage
    {
        private TextMessage message;

        public MiniKFTextMessage()
        {
            msgtype = MiniMessageType.text.ToString();
            message = new TextMessage();
        }

        public TextMessage text { get => message; set => message = value; }

        public class TextMessage
        {
            public string content { get; set; }
        }
    }

    public class MiniKFImageMessage : MiniKFMessage
    {
        private ImageMessage message;

        public MiniKFImageMessage()
        {
            msgtype = MiniMessageType.image.ToString();
            message = new ImageMessage();
        }

        public ImageMessage image { get => message; set => message = value; }

        public class ImageMessage
        {
            public string media_id { get; set; }
        }
    }

    public class MiniKFLinkMessage : MiniKFMessage
    {
        private LinkMessage message;

        public MiniKFLinkMessage()
        {
            msgtype = MiniMessageType.link.ToString();
            link = new LinkMessage();
        }

        public LinkMessage link { get => message; set => message = value; }

        public class LinkMessage
        {
            public string title { get; set; }

            public string description { get; set; }

            public string url { get; set; }

            public string thumb_url { get; set; }
        }
    }

    public class MiniKFMiniMessage : MiniKFMessage
    {
        private MiniMessage message;

        public MiniKFMiniMessage()
        {
            msgtype = MiniMessageType.miniprogrampage.ToString();
            miniprogrampage = new MiniMessage();
        }

        public MiniMessage miniprogrampage { get => message; set => message = value; }

        public class MiniMessage
        {
            public string title { get; set; }

            public string pagepath { get; set; }

            public string thumb_media_id { get; set; }
        }
    }
}
