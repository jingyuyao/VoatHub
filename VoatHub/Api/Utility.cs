using System;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using Windows.Web.Http;

using Newtonsoft.Json;

namespace VoatHub.Api
{
    public static class Utility
    {
        public static string ToQueryString(object obj)
        {
            if (obj == null) return "";

            var properties = from p in obj.GetType().GetProperties()
                        where p.GetValue(obj, null) != null
                        select nameValuePairMaker(p.Name, p.GetValue(obj, null));

            return String.Join("&", properties.ToArray());
        }

        /// <summary>
        /// Creates a name=value string. Converts Enums to int.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string nameValuePairMaker(string name, object value)
        {
            var typeEquals = typeof(Enum).IsAssignableFrom(value.GetType());
            return name + "=" + (typeEquals ? ((int)value).ToString() : value.ToString());
        }

        /// <summary>
        /// Serialize a Json object to <see cref="HttpStringContent"/>.
        /// </summary>
        /// <param name="anySerializeable"></param>
        /// <returns></returns>
        public static HttpStringContent serializeJson(object anySerializeable)
        {
            return new HttpStringContent(JsonConvert.SerializeObject(anySerializeable));
        }
    }
}
