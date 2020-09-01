using LazyWeChat.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LazyWeChat.Models
{
    public enum MessageType
    {
        Text,
        Image,
        Voice,
        Video,
        Music,
        News,
        News_Article,
        KF,
        KFJson,
        Notify,
        Encrypt,
        EncryptJson
    }

    public enum MessageFormat
    {
        Xml,
        Json
    }

    public static class MessageTemplateFactory
    {
        public static string CreateInstance(MessageType type)
        {
            var template = "";
            switch (type)
            {
                case MessageType.Text:
                    template = @"<xml>
                                  <ToUserName><![CDATA[{0}]]></ToUserName>
                                  <FromUserName><![CDATA[{1}]]></FromUserName>
                                  <CreateTime>{2}</CreateTime>
                                  <MsgType><![CDATA[text]]></MsgType>
                                  <Content><![CDATA[{3}]]></Content>
                                </xml>";
                    break;
                case MessageType.Image:
                    template = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[image]]></MsgType>
                                    <Image>
                                        <MediaId><![CDATA[{3}]]></MediaId>
                                    </Image>
                                </xml>";
                    break;
                case MessageType.Voice:
                    template = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[voice]]></MsgType>
                                    <Voice>
                                        <MediaId><![CDATA[{3}]]></MediaId>
                                    </Voice>
                                </xml>";
                    break;
                case MessageType.Video:
                    template = @"<xml>
                                <ToUserName><![CDATA[{0}]]></ToUserName>
                                <FromUserName><![CDATA[{1}]]></FromUserName>
                                <CreateTime>{2}</CreateTime>
                                <MsgType><![CDATA[video]]></MsgType>
                                <Video>
                                    <MediaId><![CDATA[media_id]]></MediaId>
                                    <Title><![CDATA[title]]></Title>
                                    <Description><![CDATA[description]]></Description>
                                </Video>
                            </xml>";
                    break;
                case MessageType.Music:
                    template = @"<xml>
                                  <ToUserName><![CDATA[{0}]]></ToUserName>
                                  <FromUserName><![CDATA[{1}]]></FromUserName>
                                  <CreateTime>{2}</CreateTime>
                                  <MsgType><![CDATA[music]]></MsgType>
                                  <Music>
                                    <Title><![CDATA[{3}]]></Title>
                                    <Description><![CDATA[{4}]]></Description>
                                    <MusicUrl><![CDATA[{5}]]></MusicUrl>
                                    <HQMusicUrl><![CDATA[{6}]]></HQMusicUrl>
                                    <ThumbMediaId><![CDATA[{7}]]></ThumbMediaId>
                                  </Music>
                                </xml>";
                    break;
                case MessageType.News:
                    template = @"<xml>
                                    <ToUserName><![CDATA[{0}]]></ToUserName>
                                    <FromUserName><![CDATA[{1}]]></FromUserName>
                                    <CreateTime>{2}</CreateTime>
                                    <MsgType><![CDATA[news]]></MsgType>
                                    <ArticleCount>{3}</ArticleCount>
                                    <Articles>
                                    {4}
                                    </Articles>
                                </xml>";
                    break;
                case MessageType.KF:
                    template = @"<xml> 
                                    <ToUserName><![CDATA[{0}]]></ToUserName>  
                                    <FromUserName><![CDATA[{1}]]></FromUserName>  
                                    <CreateTime>{2}</CreateTime>  
                                    <MsgType><![CDATA[transfer_customer_service]]></MsgType> 
                                    {3}
                                </xml>";
                    break;
                case MessageType.KFJson:
                    template = @"{{""ToUserName"":""{0}"",  
                                    ""FromUserName"":""{1}"",  
                                    ""CreateTime"":{2},  
                                    ""MsgType"":""transfer_customer_service"" 
                                    {3}
                                }}";
                    break;
                case MessageType.News_Article:
                    template = @"<item>
                                    <Title><![CDATA[{0}]]></Title>
                                    <Description><![CDATA[{1}]]></Description>
                                    <PicUrl><![CDATA[{2}]]></PicUrl>
                                    <Url><![CDATA[{3}]]></Url>
                                </item>";
                    break;
                case MessageType.Notify:
                    template = @"<xml>
                                    <return_code><![CDATA[{0}]]></return_code>
                                    <return_msg><![CDATA[{1}]]></return_msg>
                                </xml>";
                    break;
                case MessageType.Encrypt:
                    template = @"<xml>
                                    <Encrypt><![CDATA[{0}]]></Encrypt>
                                    <MsgSignature><![CDATA[{1}]]></MsgSignature>
                                    <TimeStamp><![CDATA[{2}]]></TimeStamp>
                                    <Nonce><![CDATA[{3}]]></Nonce>
                                </xml>";
                    break;
                case MessageType.EncryptJson:
                    template = @"{{""Encrypt"":""{0}"",
                                    ""MsgSignature"":""{1}"",
                                    ""TimeStamp"":{2},
                                    ""Nonce"":""{3}""}}";
                    break;
                default:
                    break;
            }
            return template;
        }
    }

    public class WeChatMessager
    {
        private string _message = "";
        /// <summary>
        /// 微信服务的类型(在配置文件中指定)
        /// </summary>
        public APIType type { get; set; }

        /// <summary>
        /// HTTP请求类型
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 微信传递的signature
        /// </summary>
        public string signature { get; set; }

        /// <summary>
        /// 微信传递的timestamp
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 微信传递的nonce
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// 微信传递的echostr
        /// </summary>
        public string echostr { get; set; }

        /// <summary>
        /// 是否为微信服务器传递的消息
        /// </summary>
        public bool validation { get; set; }

        /// <summary>
        /// 消息内容的dynamic形式
        /// </summary>
        public dynamic messageBody { get; set; }

        /// <summary>
        /// 消息格式
        /// </summary>
        public MessageFormat format { get; set; }

        /// <summary>
        /// 消息内容的文本形式
        /// </summary>
        public string message
        {
            get
            {
                var defaultMessage = string.IsNullOrEmpty(echostr) ? (method.ToLower() == "get" ? "PINGPONG" : "success") : echostr;
                if (messageBody != null && UtilRepository.IsPropertyExist(messageBody, "return_code"))
                {
                    if (messageBody.return_code == "SUCCESS")
                        defaultMessage = string.Format(MessageTemplateFactory.CreateInstance(MessageType.Notify), "SUCCESS", "OK");
                    else
                        defaultMessage = string.Format(MessageTemplateFactory.CreateInstance(MessageType.Notify),
                            messageBody.return_code, messageBody.return_msg);
                }
                if (type == APIType.MiniProgram)
                    _message = _message.Contains("transfer_customer_service") ? _message : defaultMessage;
                _message = string.IsNullOrEmpty(_message) ? defaultMessage : _message;
                return _message;
            }
        }

        /// <summary>
        /// 回复文本消息
        /// </summary>
        [JsonIgnore]
        public Action<string> replyTextMessage
        {
            get => (text) =>
                    _message = string.Format(MessageTemplateFactory.CreateInstance(MessageType.Text),
                        messageBody.FromUserName,
                        messageBody.ToUserName,
                        UtilRepository.GetUTCTicks(),
                        text);
        }

        /// <summary>
        /// 回复图片消息
        /// </summary>
        [JsonIgnore]
        public Action<string> replyImageMessage
        {
            get => (image) =>
                    _message = string.Format(MessageTemplateFactory.CreateInstance(MessageType.Image),
                        messageBody.FromUserName,
                        messageBody.ToUserName,
                        UtilRepository.GetUTCTicks(),
                        image);
        }

        /// <summary>
        /// 回复语音消息
        /// </summary>
        [JsonIgnore]
        public Action<string> replyVoiceMessage
        {
            get => (voice) =>
                    _message = string.Format(MessageTemplateFactory.CreateInstance(MessageType.Voice),
                        messageBody.FromUserName,
                        messageBody.ToUserName,
                        UtilRepository.GetUTCTicks(),
                        voice);
        }

        /// <summary>
        /// 回复视频消息
        /// </summary>
        [JsonIgnore]
        public Action<string, string, string> replyVideoMessage
        {
            get => (mediaId, title, description) =>
                    _message = string.Format(MessageTemplateFactory.CreateInstance(MessageType.Voice),
                        messageBody.FromUserName,
                        messageBody.ToUserName,
                        UtilRepository.GetUTCTicks(),
                        mediaId, title, description);
        }

        /// <summary>
        /// 回复音乐消息
        /// </summary>
        [JsonIgnore]
        public Action<string, string, string, string, string> replyMusicMessage
        {
            get => (media_id, title, description, music_url, hq_music_url) =>
                    _message = string.Format(MessageTemplateFactory.CreateInstance(MessageType.Music),
                        messageBody.FromUserName,
                        messageBody.ToUserName,
                        UtilRepository.GetUTCTicks(),
                        title, description, music_url, hq_music_url, media_id);
        }

        /// <summary>
        /// 回复图文消息
        /// </summary>
        [JsonIgnore]
        public Action<List<(string, string, string, string)>> replyNewsMessage
        {
            get => (items) =>
             {
                 if (messageBody.msgType == "text" ||
                     messageBody.msgType == "image" ||
                     messageBody.msgType == "voice" ||
                     messageBody.msgType == "video" ||
                     messageBody.msgType == "location")
                 {
                     if (items.Count > 1)
                         throw new ArgumentOutOfRangeException("articleCount should be 1 when message type is 'text' or 'image' or 'voice' or 'video' or 'location'");
                 }

                 var articles = "";
                 items.ForEach(i =>
                    articles += string.Format(MessageTemplateFactory.CreateInstance(MessageType.News_Article), i.Item1, i.Item2, i.Item3, i.Item4)
                 );

                 _message = string.Format(MessageTemplateFactory.CreateInstance(MessageType.News),
                        messageBody.FromUserName,
                        messageBody.ToUserName,
                        UtilRepository.GetUTCTicks(),
                        articles);
             };
        }

        /// <summary>
        /// 消息传递给公众号客服
        /// </summary>
        [JsonIgnore]
        public Action redirectToKF
        {
            get => () =>
            {
                _message = string.Format(MessageTemplateFactory.CreateInstance(format == MessageFormat.Xml ? MessageType.KF : MessageType.KFJson),
                    messageBody.FromUserName,
                    messageBody.ToUserName,
                    UtilRepository.GetUTCTicks(),
                    "");
            };
        }

        /// <summary>
        /// 消息传递给公众号指定客服
        /// </summary>
        [JsonIgnore]
        public Action<List<string>> redirectToSpecifiedKF
        {
            get => (items) =>
            {
                var KFList = "";
                if (items.Count > 0)
                {
                    items.ForEach(i =>
                        KFList += $"<KfAccount><![CDATA[{i}]]></KfAccount> ");
                    KFList = $"<TransInfo>{KFList}</TransInfo>";
                }

                _message = string.Format(MessageTemplateFactory.CreateInstance(format == MessageFormat.Xml ? MessageType.KF : MessageType.KFJson),
                    messageBody.FromUserName,
                    messageBody.ToUserName,
                    UtilRepository.GetUTCTicks(),
                    $",{KFList}");
            };
        }

        /// <summary>
        /// 关注事件
        /// </summary>
        /// <param name="action"></param>
        public void onSubscribeEvent(Action<string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "subscribe")
                {
                    var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";
                    var ticket = UtilRepository.IsPropertyExist(messageBody, "Ticket") ? messageBody.Ticket : "";

                    action(eventKey, ticket);
                }
            }
        }

        /// <summary>
        /// 取消关注事件
        /// </summary>
        /// <param name="action"></param>
        public void onUnsubscribeEvent(Action action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "unsubscribe")
                {
                    action();
                }
            }
        }

        /// <summary>
        /// 扫描带参数二维码事件
        /// </summary>
        /// <param name="action"></param>
        public void onScanEvent(Action<string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "SCAN")
                {
                    var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";
                    var ticket = UtilRepository.IsPropertyExist(messageBody, "Ticket") ? messageBody.Ticket : "";

                    action(eventKey, ticket);
                }
            }
        }

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        /// <param name="action"></param>
        public void onLocationEvent(Action<decimal, decimal, decimal> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "LOCATION")
                {
                    var latitude = UtilRepository.IsPropertyExist(messageBody, "Latitude") ? decimal.Parse(messageBody.Latitude) : 0;
                    var longitude = UtilRepository.IsPropertyExist(messageBody, "Longitude") ? decimal.Parse(messageBody.Longitude) : 0;
                    var precision = UtilRepository.IsPropertyExist(messageBody, "Precision") ? decimal.Parse(messageBody.Precision) : 0;

                    action(latitude, longitude, precision);
                }
            }
        }

        /// <summary>
        /// 点击自定义菜单拉取消息时的事件推送
        /// </summary>
        /// <param name="action"></param>
        public void onMenuClickEvent(Action<string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "CLICK")
                {
                    var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";

                    action(eventKey);
                }
            }
        }

        /// <summary>
        /// 点击自定义菜单跳转链接时的事件推送
        /// </summary>
        /// <param name="action"></param>
        public void onMenuViewEvent(Action<string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "VIEW")
                {
                    var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";
                    var menuId = UtilRepository.IsPropertyExist(messageBody, "MenuId") ? messageBody.MenuId : "";

                    action(eventKey, menuId);
                }
            }
        }

        /// <summary>
        /// 模板消息发送后的推送事件
        /// </summary>
        /// <param name="action"></param>
        public void onTemplateMessageSentEvent(Action<string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "TEMPLATESENDJOBFINISH")
                {
                    var msgID = UtilRepository.IsPropertyExist(messageBody, "MsgID") ? messageBody.MsgID : "";
                    var status = UtilRepository.IsPropertyExist(messageBody, "Status") ? messageBody.Status : "";
                    action(msgID, status);
                }
            }
        }

        public void onScanCodePushEvent(Action<string, string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "scancode_push")
                {
                    if (UtilRepository.IsPropertyExist(messageBody, "ScanCodeInfo"))
                    {
                        var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";
                        var scanType = messageBody.ScanCodeInfo.ScanType;
                        var scanResult = messageBody.ScanCodeInfo.ScanResult;
                        action(eventKey, scanType, scanResult);
                    }
                }
            }
        }

        public void onScancodeWaitmsgEvent(Action<string, string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "scancode_waitmsg")
                {
                    if (UtilRepository.IsPropertyExist(messageBody, "ScanCodeInfo"))
                    {
                        var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";
                        var scanType = messageBody.ScanCodeInfo.ScanType;
                        var scanResult = messageBody.ScanCodeInfo.ScanResult;
                        action(eventKey, scanType, scanResult);
                    }
                }
            }
        }

        public void onPicSysphotoEvent(Action<string, string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "pic_sysphoto")
                {
                    if (UtilRepository.IsPropertyExist(messageBody, "SendPicsInfo"))
                    {
                        //var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";
                        //var scanType = messageBody.ScanCodeInfo.ScanType;
                        //var scanResult = messageBody.ScanCodeInfo.ScanResult;
                        //action(eventKey, scanType, scanResult);
                    }
                }
            }
        }

        public void onQualificationVerifySuccess(Action<long> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "qualification_verify_success")
                {
                    long.TryParse(
                        UtilRepository.IsPropertyExist(messageBody, "ExpiredTime") ? messageBody.ExpiredTime : 0,
                        out long expiredTime);
                    action(expiredTime);
                }
            }
        }

        public void onQualificationVerifyFail(Action<long, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "qualification_verify_fail")
                {
                    long.TryParse(
                        UtilRepository.IsPropertyExist(messageBody, "FailTime") ? messageBody.FailTime : 0,
                        out long failTime);

                    var failReason = UtilRepository.IsPropertyExist(messageBody, "FailReason") ? messageBody.FailReason : "";

                    action(failTime, failReason);
                }
            }
        }

        public void onNamingVerifySuccess(Action<long> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "naming_verify_success")
                {
                    long.TryParse(
                        UtilRepository.IsPropertyExist(messageBody, "ExpiredTime") ? messageBody.ExpiredTime : 0,
                        out long expiredTime);
                    action(expiredTime);
                }
            }
        }

        public void onNamingVerifyFail(Action<long, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "naming_verify_fail")
                {
                    long.TryParse(
                        UtilRepository.IsPropertyExist(messageBody, "FailTime") ? messageBody.FailTime : 0,
                        out long failTime);

                    var failReason = UtilRepository.IsPropertyExist(messageBody, "FailReason") ? messageBody.FailReason : "";

                    action(failTime, failReason);
                }
            }
        }

        public void onAnnualRenewSuccess(Action<long> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "annual_renew")
                {
                    long.TryParse(
                        UtilRepository.IsPropertyExist(messageBody, "ExpiredTime") ? messageBody.ExpiredTime : 0,
                        out long expiredTime);
                    action(expiredTime);
                }
            }
        }

        public void onVerifyExpiredSuccess(Action<long> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "verify_expired")
                {
                    long.TryParse(
                        UtilRepository.IsPropertyExist(messageBody, "ExpiredTime") ? messageBody.ExpiredTime : 0,
                        out long expiredTime);
                    action(expiredTime);
                }
            }
        }

        public void onReceiveWxPayCallback(Action action)
        {
            //<xml><appid><![CDATA[wxbb23a029883b991d]]></appid>
            //<bank_type><![CDATA[OTHERS]]></bank_type>
            //<cash_fee><![CDATA[1]]></cash_fee>
            //<fee_type><![CDATA[CNY]]></fee_type>
            //<is_subscribe><![CDATA[Y]]></is_subscribe>
            //<mch_id><![CDATA[1501396621]]></mch_id>
            //<nonce_str><![CDATA[6c900e92fa574e939ac4d9f49d7ef644]]></nonce_str>
            //<openid><![CDATA[oNDiC0d-r7Su5mYCU-mXFSXuhmtQ]]></openid>
            //<out_trade_no><![CDATA[1234567890-58034]]></out_trade_no>
            //<result_code><![CDATA[SUCCESS]]></result_code>
            //<return_code><![CDATA[SUCCESS]]></return_code>
            //<sign><![CDATA[AE0CF7AAD003EF98BCF60A4682F379FE]]></sign>
            //<time_end><![CDATA[20200724131750]]></time_end>
            //<total_fee>1</total_fee>
            //<trade_type><![CDATA[JSAPI]]></trade_type>
            //<transaction_id><![CDATA[4200000709202007240361132851]]></transaction_id>
            //</xml>

            if (UtilRepository.IsPropertyExist(messageBody, "total_fee") &&
                UtilRepository.IsPropertyExist(messageBody, "trade_type") &&
                UtilRepository.IsPropertyExist(messageBody, "openid"))
            {
                action();
            }
        }
    }
}
