using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
namespace MCSharper
{
    public class ReserveName : AuthedEndpoint
    {
        public string name;

        public ReserveName(string user, string pass) : base(user, pass)
        {
        }
        public ReserveName(string tokenclientid) : base(tokenclientid) 
        {
        }
        //reserves a name to your account for 24 hours hours requires demo (captcha/upgraded) token
        public void reserveName()
        {
            using (wc)
            {
                method = "PUT";
                url = "https://api.mojang.com/";
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);
                wc.UploadString(url + "user/profile/agent/minecraft/name/" + name, method, "");
                wc.Headers.Remove(HttpRequestHeader.Authorization);
            }
        }

        
    }
}
