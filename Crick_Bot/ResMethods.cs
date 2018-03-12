using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crick_Bot
{
    public class ResMethods
    {
        public string getAuth_Token()
        {
            HttpReqRes res = new HttpReqRes();
            // Req to Auth API
            string json_str = res.MakeRequest(null);
            RootObject ro = JsonConvert.DeserializeObject<RootObject>(json_str);
            // getting the access token
            string access_token = ro.auth.access_token.ToString();
            return access_token;
        }

        public List<string> getSeason_KeysList()
        {
            List<string> season_keys = new List<string>();
            HttpReqRes res = new HttpReqRes();
            // Req to Auth API
            String Endpoint = "https://rest.cricketapi.com/rest/v2/recent_seasons/?access_token=" + getAuth_Token();
            string json_str = res.MakeRequest(Endpoint);
            RootObject1 ro = JsonConvert.DeserializeObject<RootObject1>(json_str);
            
            string data_str = ro.data.ToString();
            JArray data_ary = JArray.Parse(data_str);
            for (int i = 0; i < data_ary.Count(); i++)
            {
                season_keys.Add(data_ary[i]["key"].ToString());
            }
            return season_keys;
        }

        /*public List<string> getMatches_TitleList()
        {
            HttpReqRes res = new HttpReqRes();
            List<String> key = new List<string>();
                // Recent Matches API 
                HttpReqRes res1 = new HttpReqRes();
                //https://rest.cricketapi.com/rest/v2/season/" + s_keys[i] + "/recent_matches/?access_token=" + getAuth_Token()
                String Endpoint_recent_matches = "https://rest.cricketapi.com/rest/v2/recent_matches/?access_token=" + getAuth_Token();
                String json_str_recent_matches = res1.MakeRequest(Endpoint_recent_matches);
                RootObject1 ro = JsonConvert.DeserializeObject<RootObject1>(json_str_recent_matches);
                var data_str = ro.data.ToString();
                var data_obj = JObject.Parse(data_str);
                JArray card = (JArray)data_obj["cards"];
                
                //gets the keys to list
                for (int j = 0; j < card.Count(); j++)
                {
                    /*await connector.Conversations.ReplyToActivityAsync(activity.CreateReply($" Match Key : {card[i]["title"]}" + Environment.NewLine +
                        Environment.NewLine + $"{card[i]["status"]}"));
                    key.Add(card[j]["title"].ToString());
                }
            //}
            return key;
        }*/

        public string getMatch_Key(string name)
        {
            HttpReqRes res = new HttpReqRes();
            List<String> key = new List<string>();
            // Recent Matches API 
            String Endpoint_recent_matches = "https://rest.cricketapi.com/rest/v2/recent_matches/?access_token=" + getAuth_Token();
            String json_str_recent_matches = res.MakeRequest(Endpoint_recent_matches);
            RootObject1 ro = JsonConvert.DeserializeObject<RootObject1>(json_str_recent_matches);
            var data_str = ro.data.ToString();
            var data_obj = JObject.Parse(data_str);
            JArray card = (JArray)data_obj["cards"];
            string[] n = name.Split('-');
            for (int i = 0; i < card.Count(); i++)
            {
                if (card[i]["name"].ToString().Equals(n[0]) && card[i]["related_name"].ToString().Equals(n[1]))
                {
                    return card[i]["key"].ToString();
                }
            }
            return "none";
        }

        /*public List<string> getMatches_DateList()
        {
            // List<string> matches_title = new List<string>();
            // string Endpoint = "";
            HttpReqRes res = new HttpReqRes();
            List<String> key = new List<string>();
            //for (int i = 0; i < s_keys.Count(); i++)
            //{

            // Recent Matches API 
            HttpReqRes res1 = new HttpReqRes();
            //https://rest.cricketapi.com/rest/v2/season/" + s_keys[i] + "/recent_matches/?access_token=" + getAuth_Token()
            String Endpoint_recent_matches = "https://rest.cricketapi.com/rest/v2/recent_matches/?access_token=" + getAuth_Token();
            String json_str_recent_matches = res1.MakeRequest(Endpoint_recent_matches);
            RootObject1 ro = JsonConvert.DeserializeObject<RootObject1>(json_str_recent_matches);
            var data_str = ro.data.ToString();
            var data_obj = JObject.Parse(data_str);
            JArray card = (JArray)data_obj["cards"];

            //gets the keys to list
            for (int j = 0; j < card.Count(); j++)
            {
                /*await connector.Conversations.ReplyToActivityAsync(activity.CreateReply($" Match Key : {card[i]["title"]}" + Environment.NewLine +
                    Environment.NewLine + $"{card[i]["status"]}"));
                DateTime dt = DateTime.Parse(card[j]["start_date"]["iso"].ToString());
                string match_time = dt.ToLocalTime().ToShortTimeString();
                key.Add(DateTime.Parse(match_time).ToLocalTime().ToString());
            }
            //}
            return key;
        }*/

       
        /*public JObject getResponse()
        {
            HttpReqRes res = new HttpReqRes();

            // Match API
            String Endpoint_match = "https://rest.cricketapi.com/rest/v2/match/engrsa_2017_t20_01/?access_token=" + getAuth_Token();
            String json_str_match = res.MakeRequest(Endpoint_match);
            RootObject1 ro_match = JsonConvert.DeserializeObject<RootObject1>(json_str_match);
            var mdata_str = ro_match.data.ToString();
            var mdata_obj = JObject.Parse(mdata_str);

            return mdata_obj;
        }*/
        
        public List<string> getAllRecentList()
        {
            HttpReqRes res = new HttpReqRes();
            // Recent Matches API 
            String Endpoint_recent_matches = "https://rest.cricketapi.com/rest/v2/recent_matches/?access_token=" + getAuth_Token();
            String json_str_recent_matches = res.MakeRequest(Endpoint_recent_matches);
            RootObject1 ro = JsonConvert.DeserializeObject<RootObject1>(json_str_recent_matches);
            var data_str = ro.data.ToString();
            var data_obj = JObject.Parse(data_str);
            JArray card = (JArray)data_obj["cards"];
            List<String> key = new List<string>();
            //gets the keys to list
            for (int i = 0; i < card.Count(); i++)
            {
                key.Add(card[i]["key"].ToString());
            }
            List<string> recent_upcoming = new List<string>();
            List<string> recent_upcoming_status = new List<string>();
            List<string> recent_today = new List<string>();
            List<string> recent_today_status = new List<string>();
            List<string> recent_completed = new List<string>();
            List<string> recent_completed_status = new List<string>();

            for (int k = 0; k < card.Count(); k++)
            {
                // today matches
                if (DateTime.Parse(card[k]["start_date"]["iso"].ToString()).Date == DateTime.Parse(DateTime.Now.ToString()).Date)
                {
                    //recent_today.Add(card[k]["name"].ToString() + "-" + card[k]["related_name"].ToString());
                    // compare with current time & status started
                    if (DateTime.Parse(card[k]["start_date"]["iso"].ToString()).ToLocalTime() >= DateTime.Parse(DateTime.Now.ToString()))
                    {
                        recent_today.Add(card[k]["name"].ToString() + "-" + card[k]["related_name"].ToString() + "-Start at: " + DateTime.Parse(card[k]["start_date"]["iso"].ToString()).ToLocalTime());
                    }
                    else 
                    {
                        recent_today.Add(card[k]["name"].ToString() + "-" + card[k]["related_name"].ToString() + "- Match Started");
                    }

                } // completed
                else if (DateTime.Parse(card[k]["start_date"]["iso"].ToString()).Date < DateTime.Parse(DateTime.Now.ToString()).Date)
                {
                    recent_completed.Add(card[k]["name"].ToString() + "-" + card[k]["related_name"].ToString());
                } // upcoming
                else
                {
                    recent_upcoming.Add(card[k]["name"].ToString() + "-" + card[k]["related_name"].ToString());
                }
            }
            List<string> All = new List<string>();
            // adds recent to all list
            for (int v = 0; v < recent_today.Count(); v++)
            {
                All.Add(recent_today[v]);
            }
            // adds upcoming to all list
            for (int v = 0; v < recent_upcoming.Count(); v++)
            {
                All.Add(recent_upcoming[v]);
            }
            for (int v = 0; v < recent_completed.Count(); v++)
            {
                All.Add(recent_completed[v]);
            }
           /* for (int v = 0; v < All.Count(); v++)
            {
                await connector.Conversations.ReplyToActivityAsync(activity.CreateReply($" {All[v]}"));
                // All.Add(recent_today[v] + "--" + recent_today_status[v]);
            }*/
            return All;
        }
    }
}