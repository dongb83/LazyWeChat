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
    }

    public class MiniKFTextMessage : MiniKFMessage
    {
        private TextMessage message;

        public MiniKFTextMessage()
        {
            message = new TextMessage();
        }

        public string msgtype { get => MiniMessageType.text.ToString(); }

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
            message = new ImageMessage();
        }
        public string msgtype { get => MiniMessageType.image.ToString(); }

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
            link = new LinkMessage();
        }
        public string msgtype { get => MiniMessageType.link.ToString(); }

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
            miniprogrampage = new MiniMessage();
        }
        public string msgtype { get => MiniMessageType.miniprogrampage.ToString(); }

        public MiniMessage miniprogrampage { get => message; set => message = value; }

        public class MiniMessage
        {
            public string title { get; set; }

            public string pagepath { get; set; }

            public string thumb_media_id { get; set; }
        }
    }
}
