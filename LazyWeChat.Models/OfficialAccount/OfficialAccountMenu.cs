using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace LazyWeChat.Models.OfficialAccount
{
    public enum MenuType
    {
        click,
        view,
        miniprogram,
        scancode_waitmsg,
        scancode_push,
        pic_sysphoto,
        pic_photo_or_album,
        pic_weixin,
        location_select,
        media_id
    }

    public class WeChatMenu
    {
        public WeChatMenu() => sub_button = new List<WeChatMenu>();

        public string type { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public string appid { get; set; }

        public string pagepath { get; set; }

        public string key { get; set; }

        public List<WeChatMenu> sub_button { get; set; }
    }

    public class MenuButton
    {
        private List<WeChatMenu> buttons = new List<WeChatMenu>();

        public void AddMenu(WeChatMenu menu) => buttons.Add(menu);

        public string ToJson()
        {
            List<object> returnObject = GenerateJson(buttons);
            dynamic requestObject = new ExpandoObject();
            requestObject.button = returnObject;
            return JsonConvert.SerializeObject(requestObject);

            List<object> GenerateJson(List<WeChatMenu> items)
            {
                List<object> returnJson = new List<object>();
                items.ForEach(button =>
                {
                    dynamic obj = new ExpandoObject();
                    var type = button.GetType();
                    var properties = type.GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var propertyName = properties[i].Name;
                        var propertyValue = properties[i].GetValue(button);
                        var propertyType = properties[i].PropertyType.FullName;

                        if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString()))
                        {
                            if (propertyType == typeof(List<WeChatMenu>).FullName)
                            {
                                var list = propertyValue as List<WeChatMenu>;
                                if (list.Count > 0)
                                    obj.sub_button = GenerateJson(list);
                            }
                            else
                                ((IDictionary<string, object>)obj).Add(propertyName, propertyValue);
                        }
                    }
                    returnJson.Add(obj);
                });
                return returnJson;
            }
        }
    }
}
