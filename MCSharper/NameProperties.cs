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
        public NameProperties(string Name) : base(null)
        {
            name = Name;
        }
        //returns all uuids sorted from oldest to latest with optional paramter of capping the uuids (use 1 for the latest uuid)
        public List<string> grabAllUUID(int amount = Int32.MaxValue)
        {

            List<string> uuidlist = new List<string>();
            long currentepoch = DateTimeOffset.Now.ToUnixTimeSeconds();
            string uuid = "";
            using (wc)
            {
                url = "https://api.mojang.com/";
                method = "GET";
                while (currentepoch > 1420154260)
                {
                    if (uuidlist.Count >= amount)
                    {
                        break;
                    }

                    string Suuid = uuid;

                    try
                    {
                        string payload = string.Format(url + "users/profiles/minecraft/{0}?at=" + currentepoch, name);
                        string response = wc.DownloadString(payload);

                        uuid = JObject.Parse(response)["id"].ToString();
                    }
                    catch
                    {
                    }

                    currentepoch -= (long) 3196800;
                    if (uuid != Suuid)
                    {
                        uuidlist.Add(uuid);
                    }
                }

            }

            UUID = uuidlist[0]; //sets latest uuid to the classes uuid
            uuidlist.Reverse();
            return uuidlist;
        }

        //Grabs uuid at the timestamp
        public string UUIDAtTimeStamp(long UnixSeconds) 
        {
   
            using (wc)
            {
                url = "https://api.mojang.com/";
                method = "GET";
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
        //Grabs uuid at the timestamp
        public string UUIDAtTimeStamp(DateTimeOffset time)
        {
          
            long unix = time.ToUnixTimeSeconds();
            using (wc) 
            {
                url = "https://api.mojang.com/";
                method = "GET";
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
         
            using (wc) 
            {
                url = "https://account.mojang.com/available/minecraft/";
                method = "GET";
                string response = wc.DownloadString(url + name);
                Console.WriteLine(response);
                if (response == "TAKEN") { return false; }
                else { return true; }
            }
        }
        //Returns if the name is blocked whether by a user temporarily or by mojang permanently (requires token)
        public bool isBlocked() 
        {
            using (wc)
            {
                url = "https://api.mojang.com/user/profile/agent/minecraft/name/";
                method = "GET";
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
        //gets the droptime of an upcoming name will
        public long dropTime()
        {
            long drop = 0;
            using (wc)
            {

                    UUID = UUIDAtTimeStamp(DateTimeOffset.Now.ToUnixTimeSeconds()- 3196800);
                    method = "GET";
                    url = "https://api.mojang.com/user/profiles/";
                    string p = url + UUID + "/names";
                    Console.WriteLine(p);
                    string list = wc.DownloadString(p);
                    var jr = JArray.Parse(list);
                    List<JToken> jlist = new List<JToken>();
                    foreach (var uwu in jr)
                    {
                        if (uwu["name"].ToString().ToUpper() == name.ToUpper()) { jlist.Add(uwu); }
                    }
                    drop = Convert.ToInt64(jr[jr.IndexOf(jlist[jlist.Count - 1]) + 1]["changedToAt"]) + 3196800000;
             
            }
            return drop;
        }
    }
}
