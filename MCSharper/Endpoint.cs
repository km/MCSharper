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
        protected string url { get; set; }
        protected string method { get; set; }
        protected string payload;
    }
}
