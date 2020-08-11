using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LazyWeChat.Models
{
    public class LimitPropsContractResolver : DefaultContractResolver
    {
        private string[] props = null;
        private bool retain;

        public LimitPropsContractResolver(string[] props, bool retain = true)
        {
            this.props = props;
            this.retain = retain;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list = base.CreateProperties(type, memberSerialization);
            return list.Where(p =>
            {
                return retain ? props.Contains(p.PropertyName) : !props.Contains(p.PropertyName);
            }).ToList();
        }
    }
}
