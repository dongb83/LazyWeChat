using LazyWeChat.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace LazyWeChat.Models
{
    public enum QYMessageType
    {
        Text,
        Image,
        Voice,
        Video,
        News,
        News_Article,
        Notify,
    }

    public static class QYMessageTemplateFactory
    {
        public static string CreateInstance(QYMessageType type)
        {
            var template = "";
            switch (type)
            {
                case QYMessageType.Text:
                    template = @"<xml>
                                  <ToUserName><![CDATA[{0}]]></ToUserName>
                                  <FromUserName><![CDATA[{1}]]></FromUserName>
                                  <CreateTime>{2}</CreateTime>
                                  <MsgType><![CDATA[text]]></MsgType>
                                  <Content><![CDATA[{3}]]></Content>
                                </xml>";
                    break;
                case QYMessageType.Image:
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
                case QYMessageType.Voice:
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
                case QYMessageType.Video:
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
                case QYMessageType.News:
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
                case QYMessageType.News_Article:
                    template = @"<item>
                                    <Title><![CDATA[{0}]]></Title>
                                    <Description><![CDATA[{1}]]></Description>
                                    <PicUrl><![CDATA[{2}]]></PicUrl>
                                    <Url><![CDATA[{3}]]></Url>
                                </item>";
                    break;
                case QYMessageType.Notify:
                    template = @"<xml>
                                    <return_code><![CDATA[{0}]]></return_code>
                                    <return_msg><![CDATA[{1}]]></return_msg>
                                </xml>";
                    break;
                default:
                    break;
            }
            return template;
        }
    }

    public class WeChatQYMessager
    {
        private string _message = "";
        private string _encodingAESKey = "";

        /// <summary>
        /// 微信服务的类型(在配置文件中指定)
        /// </summary>
        public APIType type { get; set; }

        /// <summary>
        /// HTTP请求类型
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 微信传递的msg_signature
        /// </summary>
        public string msg_signature { get; set; }

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

        private string echostrDecrypted
        {
            get
            {
                if (method.ToLower() == HttpMethod.Get.ToString().ToLower() && !string.IsNullOrEmpty(echostr))
                    return Cryptography.AES_decrypt(echostr, _encodingAESKey);
                else
                    return string.Empty;
            }
        }

        public string encodingAESKey { set => _encodingAESKey = value; }

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
                var defaultMessage = string.IsNullOrEmpty(echostrDecrypted) ? (method.ToLower() == "get" ? "PINGPONG" : "success") : echostrDecrypted;
                if (messageBody != null && UtilRepository.IsPropertyExist(messageBody, "return_code"))
                {
                    if (messageBody.return_code == "SUCCESS")
                        defaultMessage = string.Format(QYMessageTemplateFactory.CreateInstance(QYMessageType.Notify), "SUCCESS", "OK");
                    else
                        defaultMessage = string.Format(QYMessageTemplateFactory.CreateInstance(QYMessageType.Notify),
                            messageBody.return_code, messageBody.return_msg);
                }
                _message = string.IsNullOrEmpty(_message) ? defaultMessage : _message;
                return _message;
            }
        }

        #region 消息回复
        /// <summary>
        /// 回复文本消息
        /// </summary>
        [JsonIgnore]
        public Action<string> replyTextMessage
        {
            get => (text) =>
                    _message = string.Format(QYMessageTemplateFactory.CreateInstance(QYMessageType.Text),
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
                    _message = string.Format(QYMessageTemplateFactory.CreateInstance(QYMessageType.Image),
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
                    _message = string.Format(QYMessageTemplateFactory.CreateInstance(QYMessageType.Voice),
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
                    _message = string.Format(QYMessageTemplateFactory.CreateInstance(QYMessageType.Voice),
                        messageBody.FromUserName,
                        messageBody.ToUserName,
                        UtilRepository.GetUTCTicks(),
                        mediaId, title, description);
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

        #endregion

        #region App事件
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
        /// 进入企业微信应用时事件
        /// </summary>
        /// <param name="action"></param>
        public void onEnterAgentEvent(Action action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "enter_agent")
                {
                    action();
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
        /// 异步任务完成事件
        /// </summary>
        /// <param name="action"></param>
        public void onAsyncJobFinishedEvent(Action<string, string, string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "batch_job_result")
                {
                    if (UtilRepository.IsPropertyExist(messageBody, "BatchJob"))
                    {
                        var jobId = messageBody.JobId;
                        var jobType = messageBody.JobType;
                        var errCode = messageBody.ErrCode;
                        var errMsg = messageBody.ErrMsg;

                        action(jobId, jobType, errCode, errMsg);
                    }
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
                if (messageBody.Event == "click")
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
        public void onMenuViewEvent(Action<string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "view")
                {
                    var eventKey = UtilRepository.IsPropertyExist(messageBody, "EventKey") ? messageBody.EventKey : "";

                    action(eventKey);
                }
            }
        }

        /// <summary>
        /// 扫码推事件的事件推送
        /// </summary>
        /// <param name="action"></param>
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

        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框的事件推送
        /// </summary>
        /// <param name="action"></param>
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
        #endregion

        #region 通讯录事件
        /// <summary>
        /// 新增成员事件
        /// </summary>
        /// <param name="action"></param>
        public void onContactCreateUserEvent(Action<dynamic> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "change_contact")
                {
                    if (messageBody.ChangeType == "create_user")
                    {
                        action(messageBody);
                    }
                }
            }
        }

        /// <summary>
        /// 更新成员事件
        /// </summary>
        /// <param name="action"></param>
        public void onContactUpdateUserEvent(Action<dynamic> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "change_contact")
                {
                    if (messageBody.ChangeType == "update_user")
                    {
                        action(messageBody);
                    }
                }
            }
        }

        /// <summary>
        /// 删除成员事件
        /// </summary>
        /// <param name="action"></param>
        public void onContactDeleteUserEvent(Action<string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "change_contact")
                {
                    if (messageBody.ChangeType == "delete_user")
                    {
                        var userid = UtilRepository.IsPropertyExist(messageBody, "UserID") ? messageBody.UserID.ToString() : "";

                        action(userid);
                    }
                }
            }
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="action"></param>
        public void onContactCreatePartyEvent(Action<int, string, string, int> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "change_contact")
                {
                    if (messageBody.ChangeType == "create_party")
                    {
                        var id = UtilRepository.IsPropertyExist(messageBody, "Id") ? int.Parse(messageBody.Id) : 0;
                        var name = UtilRepository.IsPropertyExist(messageBody, "Name") ? messageBody.Name.ToString() : "";
                        var parentid = UtilRepository.IsPropertyExist(messageBody, "ParentId") ? messageBody.ParentId.ToString() : "";
                        var order = UtilRepository.IsPropertyExist(messageBody, "Order") ? int.Parse(messageBody.Order) : 0;

                        action(id, name, parentid, order);
                    }
                }
            }
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="action"></param>
        public void onContactUpdatePartyEvent(Action<int, string, string> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "change_contact")
                {
                    if (messageBody.ChangeType == "update_party")
                    {
                        var id = UtilRepository.IsPropertyExist(messageBody, "Id") ? int.Parse(messageBody.Id) : 0;
                        var name = UtilRepository.IsPropertyExist(messageBody, "Name") ? messageBody.Name.ToString() : "";
                        var parentid = UtilRepository.IsPropertyExist(messageBody, "ParentId") ? messageBody.ParentId.ToString() : "";

                        action(id, name, parentid);
                    }
                }
            }
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="action"></param>
        public void onContactDeletePartyEvent(Action<int> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "change_contact")
                {
                    if (messageBody.ChangeType == "delete_party")
                    {
                        var id = UtilRepository.IsPropertyExist(messageBody, "Id") ? int.Parse(messageBody.Id) : 0;

                        action(id);
                    }
                }
            }
        }

        /// <summary>
        /// 标签成员变更事件
        /// </summary>
        /// <param name="action"></param>
        public void onContactUpdateTagEvent(Action<List<string>, List<string>, List<string>, List<string>> action)
        {
            if (messageBody.MsgType == "event")
            {
                if (messageBody.Event == "change_contact")
                {
                    if (messageBody.ChangeType == "update_tag")
                    {
                        string addUserItems = UtilRepository.IsPropertyExist(messageBody, "AddUserItems") ? messageBody.AddUserItems.ToString() : "";
                        var addUserList = addUserItems.Split(',').ToList();


                        string delUserItems = UtilRepository.IsPropertyExist(messageBody, "DelUserItems") ? messageBody.DelUserItems.ToString() : "";
                        var delUserList = delUserItems.Split(',').ToList();


                        string addPartyItems = UtilRepository.IsPropertyExist(messageBody, "AddPartyItems") ? messageBody.AddPartyItems.ToString() : "";
                        var addPartyList = addPartyItems.Split(',').ToList();


                        string delPartyItems = UtilRepository.IsPropertyExist(messageBody, "DelPartyItems") ? messageBody.DelPartyItems.ToString() : "";
                        var delPartyList = delPartyItems.Split(',').ToList();
                        action(addUserList, delUserList, addPartyList, delPartyList);
                    }
                }
            }
        }
        #endregion
    }
}
