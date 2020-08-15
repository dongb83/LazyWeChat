namespace LazyWeChat.Models.OfficialAccount
{
    public class ArticleModel
    {
        /// <summary>
        /// 标题(必填)
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 图文消息的封面图片素材id(必须是永久mediaID)(必填)
        /// </summary>
        public string thumb_media_id { get; set; }

        /// <summary>
        /// 作者(非必填)
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 图文消息的摘要(非必填)
        /// </summary>
        public string digest { get; set; }

        /// <summary>
        /// 是否显示封面(必填), 1或者0
        /// </summary>
        public string show_cover_pic { get; set; }

        /// <summary>
        /// 图文消息的具体内容，支持HTML标签(必填)
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 图文消息的原文地址，即点击“阅读原文”后的URL(必填)
        /// </summary>
        public string content_source_url { get; set; }

        /// <summary>
        /// 是否打开评论,0不打开,1打开(非必填)
        /// </summary>
        public int need_open_comment { get; set; }

        /// <summary>
        /// 是否粉丝才可评论,0所有人可评论,1粉丝才可评论(非必填)
        /// </summary>
        public int only_fans_can_comment { get; set; }
    }
}
