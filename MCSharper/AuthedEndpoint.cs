using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;

namespace MCSharper
{
    public class AuthedEndpoint : Endpoint
    {
        public string Token;
        public string ClientID;
        public string user;
        public string pass;

        public AuthedEndpoint(string User, string Pass) 
        {
            wc = new WebClient();
            url = "https://authserver.mojang.com";
            method = "POST";
            user = User;
            pass = Pass;
        }
        //token:clientid format or it throws an exception, clientid doesnt have to be working unless your if your gonna refresh
        public AuthedEndpoint(string tokenclientid)
        {
                url = "https://authserver.mojang.com";
                method = "POST";
                Token = tokenclientid.Split(':')[0];
                ClientID = tokenclientid.Split(':')[1];
        }
        //authenticates with the optional captcha paramter, throws expcetion if it fails
        public void Authenticate(string captcha = null) 
        {
            if (pass != null)
            {
                if (captcha == null) { 
                  payload = "{\"username\":\"" + user + "\",\"password\":\"" + pass + "\",\"requestUser\":true}"; 
                 } 
                 else 
                 { 
                 payload = "{\"username\":\"" + user + "\",\"password\":\"" + pass + "\",\"requestUser\":true, \"captcha\": \"" + captcha + "\"" + ", \"captchaSupported\": \"InvisibleReCAPTCHA\"}"; 
                 
                }
                
                string response = wc.UploadString(url + "/authenticate", method, payload);
                    
                    if (response != "") { 
                         info = JObject.Parse(response); setVariables(info);    
                  }
            }
        }
        //returns true if the token is valid
        public bool isTokenValid()
        {
            payload = "{\"accessToken\":\"" + Token + "\"}";
            try
            {
                string response = wc.UploadString(url + "/validate", method, payload);
                return true;
            }
            catch { return false; }
        }
        //refreshes token
        public void refreshToken() 
        {
            payload = "{\"accessToken\":\"" + Token + "\", \"clientToken\":\"" + ClientID + "\"}";
            try 
            {
                string response = wc.UploadString(url + "/refresh", method, payload);
                setVariables(JObject.Parse(response));
            }
            catch 
            {
                throw FailedAuth("Invalid Token/ClientID");
            }
        }
        //Converts a non captcha token to a captcha token without requiring captcha for approximatly one minute
        public void UpgradeToken() 
        {
            try
            {
                wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);
                string response = wc.DownloadString("https://api.mojang.com/user/security/challenges");
                wc.Headers.Remove(HttpRequestHeader.Authorization);
            }
            catch { throw FailedAuth("Invalid Token"); }
        }
        private void setVariables(JObject j)
        {
            Token = j["accessToken"].ToString();
            ClientID = j["clientToken"].ToString();
        }
    }
}
