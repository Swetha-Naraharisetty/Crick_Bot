using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crick_Bot
{
    public class CricketAPI_Classes
    {
    }
    public class PushServer
    {
        public string host { get; set; }
        public string port { get; set; }
    }

    public class Auth
    {
        public string access_token { get; set; }
        public List<PushServer> push_servers { get; set; }
        public string expires { get; set; }
    }

    public class RootObject
    {
        public bool status { get; set; }
        public string version { get; set; }
        public int status_code { get; set; }
        public string expires { get; set; }
        public Auth auth { get; set; }
        public object Etag { get; set; }
        public string cache_key { get; set; }
    }

    public class RootObject1
    {
        public bool status { get; set; }
        public int status_code { get; set; }
        public string status_msg { get; set; }
        public string version { get; set; }
        public object data { get; set; }
    }
}