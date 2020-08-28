using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace LazyWeChat.Models.QY
{
    public class MemberAttrModel
    {
        public int type { get; set; }

        public string name { get; set; }
    }

    public class TextMemberAttrModel : MemberAttrModel
    {
        public TextMemberAttrModel() => text = new TextModel();

        public TextModel text { get; set; }

        public class TextModel
        {
            public string value { get; set; }
        }
    }

    public class WebMemberAttrModel : MemberAttrModel
    {
        public WebMemberAttrModel() => web = new WebModel();

        public WebModel web { get; set; }

        public class WebModel
        {
            public string url { get; set; }

            public string title { get; set; }
        }
    }

    public class MiniMemberAttrModel : MemberAttrModel
    {
        public MiniMemberAttrModel() => miniprogram = new MiniModel();

        public MiniModel miniprogram { get; set; }

        public class MiniModel
        {
            public string appid { get; set; }

            public string pagepath { get; set; }

            public string title { get; set; }
        }
    }

    public class MemberModel
    {
        public MemberModel()
        {
            extattr = new ExternalAttr();
            external_profile = new ExternalProfile();
        }

        public string userid { get; set; }

        public string name { get; set; }

        public string alias { get; set; }

        public string mobile { get; set; }

        public int[] department { get; set; }

        public int[] order { get; set; }

        public string position { get; set; }

        public string gender { get; set; }

        public string email { get; set; }

        public int[] is_leader_in_dept { get; set; }

        public int? enable { get; set; }

        public string avatar_mediaid { get; set; }

        public string telephone { get; set; }

        public string address { get; set; }

        public int? main_department { get; set; }

        public ExternalAttr extattr { get; set; }

        public class ExternalAttr
        {
            public ExternalAttr() => attrs = new List<MemberAttrModel>();

            public List<MemberAttrModel> attrs { get; set; }
        }

        public bool? to_invite { get; set; }

        public string external_position { get; set; }

        public ExternalProfile external_profile { get; set; }

        public class ExternalProfile
        {
            public ExternalProfile() => external_attr = new List<MemberAttrModel>();

            public string external_corp_name { get; set; }

            public List<MemberAttrModel> external_attr { get; set; }

        }

        public dynamic ToDynamic()
        {
            if (string.IsNullOrEmpty(userid))
                throw new ArgumentNullException(nameof(userid));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            dynamic obj = new ExpandoObject();
            Type type = this.GetType();
            var properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var propertyName = properties[i].Name;
                var propertyValue = properties[i].GetValue(this);

                if (propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString()))
                {
                    var serialize = true;
                    if (propertyValue is ExternalAttr externalAttr)
                        serialize = !(externalAttr.attrs.Count == 0);

                    if (propertyValue is ExternalProfile externalProfile)
                        serialize = !(string.IsNullOrEmpty(externalProfile.external_corp_name));

                    if (serialize)
                        ((IDictionary<string, object>)obj).Add(propertyName, propertyValue);
                }
            }
            return obj;
        }
    }
}
