using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galileo.Utils
{
    public class JsonHelper
    {
        public string Content { get; set; }

        public JsonHelper(string content)
        {
            Content = content;
        }
        public string GetValue(string route)
        {
            try
            {
                JObject json = JObject.Parse(Content);

                var parts = route.Split("/");

                if (parts.Length == 1)
                {
                    return (string)json[parts[0]];
                }
                else if (parts.Length == 2)
                {
                    string value = (string)json[parts[0]][parts[1]];
                    return value;
                }
                else if (parts.Length == 3)
                {
                    return (string)json[parts[0]][parts[1]][parts[2]];
                }
                else if (parts.Length == 3)
                {
                    return (string)json[parts[0]][parts[1]][parts[2]][parts[3]];
                }
                else
                    return null;
            }
            catch 
            {
                return route;
            }
        }
    }
}
