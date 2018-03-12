using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Crick_Bot
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class HttpReqRes
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public HttpReqRes()
        {

            Method = HttpVerb.GET;
            PostData = "";
        }


        public String MakeRequest(String api)
        {
            String res_str = "";
            if (api == null)
                EndPoint = "https://rest.cricketapi.com/rest/v2/auth/?access_key=b0e06a04e16b02ceb452dcbc09880322&secret_key=9905e339945fbe4fef984187c8df42ff&app_id=cricbot&device_id=s2m";
            else
                EndPoint = api;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EndPoint);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = Method.ToString();
            using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
            {
                using (System.IO.Stream res_stream = res.GetResponseStream())
                {
                    if (res_stream != null)
                    {
                        StreamReader read = new StreamReader(res_stream);
                        res_str = read.ReadToEnd();
                    }
                }
            }
            return res_str;
        }

    }
}