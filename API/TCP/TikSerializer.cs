using System.Reflection;
using Poushec.Mikrotik.Models;
using Poushec.Mikrotik.Models.IP;

namespace Poushec.Mikrotik.API.TCP
{
    internal static class TikSerializer
    {
        /// <summary>
        /// Deserializes Mikrotik replies to list of objects of T type. 
        /// 
        /// If property name differs from Mikrotik's notation - you should specify 
        /// the name by addind 'JsonPropertyName' attribute to this property.
        /// 
        /// If you want property to be ingored by deserializer - add 'JsonIgnore'
        /// attribute to it
        /// </summary>
        /// <param name="tikReply">Reply from Mikotik (output of Session.Read())</param>
        /// <typeparam name="T">Type of object to convert to</typeparam>
        /// <returns>List<T> of deserialized objects</returns>
        public static List<T> Deserialize<T>(List<string> tikReply)
        {
            if (tikReply[0] == "!done")
            {
                throw new Exception("Reply message does not contains any objects");
            }

            var result = new List<T>();

            foreach (var replyMessage in tikReply)
            {
                if (replyMessage == "!done")
                {
                    break;
                }

                var propList = replyMessage.Replace("!re=", "").Split("=").ToList();
                var tInstance = (T)Activator.CreateInstance(typeof(T));

                foreach (var prop in typeof(T).GetProperties())
                {
                    if (prop.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() != null)
                    {
                        continue;
                    }

                    var propInfo = tInstance.GetType().GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);
                    var propName = propInfo.Name;

                    var jsonAttribute = propInfo.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>();
                    
                    if (jsonAttribute != null)
                    {
                        propName = jsonAttribute.Name;
                    }

                    if (propInfo != null && propInfo.CanWrite)
                    {
                        string pValue;
                        pValue = propList[propList.IndexOf(propName.ToLower()) + 1];
                        
                        if (pValue == propList[0])
                        {
                            continue;
                        }

                        propInfo.SetValue(tInstance, pValue, null);
                    }
                }

                result.Add(tInstance);
            }

            return result;
        }
    }
}