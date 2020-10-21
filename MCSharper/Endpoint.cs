using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;

namespace MCSharper
{
    public class Endpoint
    {
        protected JObject info;
        protected WebClient wc;
        protected WebProxy wp;
        protected string url { get; set; }
        protected string method { get; set; }
        protected string payload;
        public string proxy;
        public string port;
        public string proxyUserpass;

        //must have atleast proxy and port
        public void enableProxy()
        {
            wp = new WebProxy("https://"+proxy+":"+port+"/");
            try
            {
                wp.Credentials = new NetworkCredential(proxyUserpass.Split(':')[0], proxyUserpass.Split(':')[1]);
            }
            catch 
            {
                
            }
            wc.Proxy = wp;
        }

        public void disableProxy()
        {
            wc.Proxy = null;
        }

    }
}
