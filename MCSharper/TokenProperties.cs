using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MCSharper
{
    public class TokenProperties : AuthedEndpoint
    {
        public TokenProperties(string tokenclienttoken) : base(tokenclienttoken)
        {
            try
            {
                initialize();
            }
            catch 
            {
                
            }
        }
        //requires token with minecraft otherwise you can only use email function
        private void initialize()
        {
            UpgradeToken();
            using (wc)
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);
                string response = wc.DownloadString("https://api.mojang.com/user/profiles/agent/minecraft");
                wc.Headers.Remove(HttpRequestHeader.Authorization);
                info = JObject.Parse(response.Replace("[", "").Replace("]", ""));
            }
        }

        public string getName()
        { 
            return info["name"].ToString();
        }

        public string getUUID()
        {
            return info["id"].ToString();
        }

        public string getEmail()
        {
            UpgradeToken();
            using (wc)
            {
                method = "GET";
                url = "https://api.mojang.com/user";
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);
                string response = wc.DownloadString(url);
                wc.Headers.Remove(HttpRequestHeader.Authorization);
                return JObject.Parse(response)["email"].ToString();
            }
        }

        public long getCreationDate()
        {
            return Convert.ToInt64(info["createdAt"]);
        }

        public bool isPaid()
        {
            return Convert.ToBoolean(info["paid"]);
        }

        public bool isUnmigrated()
        {
            return Convert.ToBoolean(info["legacyProfile"]);
        }
    }
}
