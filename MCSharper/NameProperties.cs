using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace MCSharper
{
    public class NameProperties : AuthedEndpoint
    {
        public string name;
        public string UUID;
        public bool taken;
        public NameProperties(string Name) : base(null)
        {
            name = Name;
        }
        //returns all uuids sorted from oldest to latest with optional paramter of capping the uuids (use 1 for the latest uuid)
        public List<string> grabUUID(int amount = 1)
        {
            method = "GET";
            url = "https://api.mojang.com/";
            List<string> uuidlist = new List<string>();
            long currentepoch = DateTimeOffset.Now.ToUnixTimeSeconds();
            string uuid = "";
            using (wc)
            {
                while (currentepoch > 1420154260)
                {
                    if (uuidlist.Count >= amount) { break; }
                    string Suuid = uuid;
                    string payload = string.Format(url+ "users/profiles/minecraft/{0}?at=" + currentepoch, name);
                    string response = wc.DownloadString(payload);
                    try
                    {
                        uuid = JObject.Parse(response)["id"].ToString();
                    }
                    catch { }
                    currentepoch -= (long)3196800;
                    if (uuid != Suuid) { uuidlist.Add(uuid); }
                }

            }
            UUID = uuidlist[0]; //sets latest uuid to the classes uuid
            uuidlist.Reverse();
            return uuidlist;
        }
        public string UUIDAtTimeStamp(long UnixSeconds) 
        {
            method = "GET";
            url = "https://api.mojang.com/";
            using (wc)
            {
                string payload = string.Format(url + "users/profiles/minecraft/{0}?at=" + UnixSeconds, name);
                string response = wc.DownloadString(payload);
                try
                {
                    UUID = JObject.Parse(response)["id"].ToString();
                }
                catch { }
            }

            return UUID;
        }
        public string UUIDAtTimeStamp(DateTimeOffset time)
        {
            method = "GET";
            url = "https://api.mojang.com/";
            long unix = time.ToUnixTimeSeconds();
            using (wc) 
            {
                string payload = string.Format(url + "users/profiles/minecraft/{0}?at=" + unix, name);
                string response = wc.DownloadString(payload);
                try
                {
                    UUID = JObject.Parse(response)["id"].ToString();
                }
                catch { }
            }

            return UUID;
        }
        //Checks if the name is available
        public bool isAvailable() 
        {
            url = "https://account.mojang.com/available/minecraft/";
            method = "GET";
            using (wc) 
            {
                string response = wc.DownloadString(url + name);
                if (response == "TAKEN") { return false; }
                else { return true; }
            }
        }
        //Returns if the name is blocked whether by a user temporarily or by mojang permanently (requires token)
        public bool isBlocked() 
        {
            url = "https://api.mojang.com/user/profile/agent/minecraft/name/";
            method = "GET";
            using (wc)
            {
                try
                {
                    wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);
                    string response = wc.DownloadString(url + name);
                    if (response == "") { return true; }
                    else { return false; }
                }
                catch { return false; }
            }
        }
    }
}
