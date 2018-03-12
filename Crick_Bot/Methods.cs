using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crick_Bot
{
    public class Methods
    {
        HttpReqRes res = new HttpReqRes();
       static  string  match_key;
        public void setMatchKey(string key)
        {
            match_key = key;    
        }
        public string getMatchKey(string key)
        {
            return match_key;
        }
        // get Data from MAtch API
        public JObject getData() {
            ResMethods rs = new ResMethods();
            String Endpoint = "https://rest.cricketapi.com/rest/v2/match/" +match_key+ "/?access_token=" + rs.getAuth_Token();
            String res_str_json = res.MakeRequest(Endpoint);
            RootObject1 ro1 = JsonConvert.DeserializeObject<RootObject1>(res_str_json);
            var schedule_str = ro1.data.ToString();
           return JObject.Parse(schedule_str);
        }
       
        /*public string getAuth_Token()
        {
            // Re to Auth API
            string json_str = res.MakeRequest("https://rest.cricketapi.com/rest/v2/auth/?access_key=b0e06a04e16b02ceb452dcbc09880322&secret_key=9905e339945fbe4fef984187c8df42ff&app_id=cricbot&device_id=swetha");
            RootObject ro = JsonConvert.DeserializeObject<RootObject>(json_str);
            // getting the access token
            string access_token = ro.auth.access_token.ToString();

            return access_token;
        }*/
        // gets Toss results
        public string GetToss()
        {
            //Recent matches Api data access
            var obj = getData();
            return obj["card"]["toss"]["str"].ToString();
        }
        // gets the current bowler name
        public string getCurrBowler()
        {
            //Recent matches Api data access
            var obj = getData();
            return getPlayerName(obj["card"]["now"]["bowler"].ToString());
        }
        // gets the openers name of the current match
        public JArray getOpeners() {
            //Recent matches Api data access
            var obj = getData();
            var batTeam = getBatTeam();
            var innings = getInnings();
            JArray battingTeam = (JArray)obj["card"]["innings"][batTeam + "_" + innings]["batting_order"];
            JArray openers = new JArray();
            openers.Add(getPlayerName(battingTeam[0].ToString()));
            openers.Add(getPlayerName(battingTeam[1].ToString()));
            string patnership = obj["card"]["innings"][batTeam + "_" + innings]["partnerships"][0]["runs"].ToString();
            openers.Add(patnership);
            return openers;
        }
        // gets the current players of the batting team
        public JArray getCurrPlayers()
        {
            var obj = getData();
            JArray currPlayers = new JArray();
            var striker = (string)obj["card"]["now"]["striker"];       // who was batting
            var nonstriker = (string)obj["card"]["now"]["nonstriker"]; // supporting batsman
            currPlayers.Add(getPlayerName(striker));
            currPlayers.Add(getPlayerName(nonstriker));
            return currPlayers;
        }
        // gets the highest score batsman among the completed batsman
        public string getHighestBatsman(string batTeam)
        {
            var obj = getData();
            JArray batOrder = getBattingOrder(batTeam.ToString());
            int highScore = 0;
            var player = "";
            for (int i = 0; i < batOrder.Count(); i++) {
                int score = (int)obj["card"]["players"][batOrder[i].ToString()]["match"]["innings"]["1"]["batting"]["runs"];
                if (score > highScore)
                {
                    highScore = score;
                    player = batOrder[i].ToString();
                }
            }
            player = getPlayerName(player);
            var result = player + "has scored the highest of " + highScore + "runs - " +getTeam(batTeam.ToString());
            return result;
        }
        // gets the original team name
        public string getTeam(string team_key) {
            var obj = getData();
            return obj["card"]["teams"][team_key]["name"].ToString();
        }
        // gets the bowling team name
        public string getBowlTeam() {
            var obj = getData();
            return (string)obj["card"]["now"]["bowling_team"];
        }
        // gets the highest scores bowlers among the completed bowlers
        public string getHighWBowler(string bowlTeam)
        {
            var obj = getData();
            JArray bowlOrder = getBowlingOrder(bowlTeam);
            int highWickets = 0;
            var player = "";
            for (int i = 0; i < bowlOrder.Count(); i++)
            {
                int wickets = (int)obj["card"]["players"][bowlOrder[i].ToString()]["match"]["innings"]["1"]["bowling"]["wickets"];
                if (wickets > highWickets)
                {
                    highWickets = wickets;
                    player = bowlOrder[i].ToString();
                }
            }
            player = getPlayerName(player);
            var result = player + "has drawn the highest of  " + highWickets + " wickets " + getTeam(bowlTeam.ToString());
            return result;
        }
        // gets the batting order along with scores
        public JArray getBatOrderScores(string team)
        {
            var obj = getData();
            JArray batOrder = getBattingOrder(team);
            JArray batOrderScores = new JArray();
            for (int i = 0; i < batOrder.Count(); i++)
            { 
                int score = (int)obj["card"]["players"][batOrder[i]]["match"]["innings"][1]["batting"]["runs"];
                batOrderScores[i] = getPlayerName(batOrder[i].ToString()) + " - " + score;
            }
            return batOrderScores;
        }
        // gets the bowling order along with the wickets
        public JArray getBowlOrderWickets(string team)
        {
            var obj = getData();
            JArray bowlOrder = getBowlingOrder(team);
            JArray bowlOrderWickets = new JArray();
            for (int i = 0; i < bowlOrder.Count(); i++)
            {
                int score = (int)obj["card"]["players"][bowlOrder[i]]["match"]["innings"][1]["bowling"]["wickets"];
                bowlOrderWickets[i] = getPlayerName(bowlOrder[i].ToString()) + " - " + score;
            }
            return bowlOrderWickets;
        }
        // gets batting order
        public JArray getBattingOrder(string team) {
            var obj = getData();
            JArray batOrder = (JArray)obj["card"]["innings"][team + "_" + getInnings()]["batting_order"];
            return batOrder;
        }
        // gets bowling order
        public JArray getBowlingOrder(string team)
        {
            var obj = getData();
            JArray bowlOrder = (JArray)obj["card"]["innings"][team + "_" + getInnings()]["bowling_order"];
            return bowlOrder;
        }
        // gets the venue
        public string getMatchVenue() {
            var obj = getData();
            return obj["card"]["venue"].ToString();
        }
        // gets the status
        public string getStatus() {
            var obj = getData();
            return obj["card"]["status"].ToString();
        }
        // gets the man of the match
        public string getManOfTheMatch()
        {
            var obj = getData();
            if (getStatus().Equals("completed"))
            {
                var man = obj["card"]["man_of_match"].ToString();
                if (man.Equals(""))
                    return  "As it is a series it does not have man of the match until the total series gets completed ";
                else
                    return obj["card"]["man_of_match"].ToString() + "is the man of the match";
            }
            else
                return "match not yet completed";
        }
        
        public string getCurrScore()
        {
            var obj = getData();    
            return obj["card"]["now"]["runs"].ToString();
           
        }
        public JArray getStrikeRate(string message)
        {
            var obj = getData();
            //var striker = (string)obj["card"]["now"]["striker"];

            JArray b_order = getBattingOrder("b");
            JArray a_order = (getBattingOrder("a"));
            JArray result = new JArray();
            string batsman = "";
            if (getPlayerId(message, a_order) == null)
            {
                if (getPlayerId(message, b_order) == null)
                {
                    result.Add(" You have spelt the player name wrong or " + Environment.NewLine + Environment.NewLine + " he/she is not a part of this match or " + Environment.NewLine + Environment.NewLine + " he/she havent played yet");
                    return result;
                }
                batsman = getPlayerId(message, b_order);

                result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["strike_rate"].ToString());
                result.Add(getPlayerName(batsman));
                return result;

            }
            batsman = getPlayerId(message, a_order);
            result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["strike_rate"].ToString());
            result.Add(getPlayerName(batsman));
            return result;

        }
        public string getPlayerId(string key, JArray order)
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] words = key.Split(delimiterChars);
            foreach (string s in words)
            {
                for (int i = 0; i < order.Count(); i++)
                {
                    // await (($"res {i.ToString()}"));
                    if (order.Count() != 0)
                    {
                        string player = getPlayerName(order[i].ToString()).ToLower();

                        if (player.Contains(s.ToLower()))
                        {
                            return order[i].ToString();
                        }
                    }
                }
            }
            return null;
        }

        public JArray getPlayerSixes(string message)
        {
            var obj = getData();
            JArray b_order = getBattingOrder("b");
            JArray a_order = (getBattingOrder("a"));
            JArray result = new JArray();
            string batsman = "";
            if (getPlayerId(message, a_order) == null)
            {
                if (getPlayerId(message, b_order) == null)
                {
                    result.Add(" You have spelt the player name wrong or " + Environment.NewLine + Environment.NewLine + " he/she is not a part of this match or " + Environment.NewLine + Environment.NewLine + " he/she havent played yet");
                    return result;
                }
                batsman = getPlayerId(message, b_order);

                result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["sixes"].ToString());
                result.Add(getPlayerName(batsman));
                return result;

            }
            batsman = getPlayerId(message, a_order);
            result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["sixes"].ToString());
            result.Add(getPlayerName(batsman));
            return result;

        }
        public JArray getPlayerDotBalls(string message)
        {
            var obj = getData();
            JArray b_order = getBattingOrder("b");
            JArray a_order = (getBattingOrder("a"));
            JArray result = new JArray();
            string batsman = "";
            if (getPlayerId(message, a_order) == null)
            {
                if (getPlayerId(message, b_order) == null)
                {
                    result.Add(" You have spelt the player name wrong or " + Environment.NewLine + Environment.NewLine + " he/she is not a part of this match or " + Environment.NewLine + Environment.NewLine + " he/she havent played yet");
                    return result;
                }
                batsman = getPlayerId(message, b_order);

                result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["dotballs"].ToString());
                result.Add(getPlayerName(batsman));
                return result;

            }
            batsman = getPlayerId(message, a_order);
            result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["dotballs"].ToString());
            result.Add(getPlayerName(batsman));
            return result;

        }
        public JArray getPlayerScore(string message)
        {
            var obj = getData();
            JArray b_order = getBattingOrder("b");
            JArray a_order = (getBattingOrder("a"));
            JArray result = new JArray();
            string batsman = "";
            if (getPlayerId(message, a_order) == null)
            {
                if (getPlayerId(message, b_order) == null)
                {
                    result.Add(" You have spelt the player name wrong or " + Environment.NewLine + Environment.NewLine + " he/she is not a part of this match or " + Environment.NewLine + Environment.NewLine + " he/she havent played yet");
                    return result;
                }
                batsman = getPlayerId(message, b_order);

                result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["runs"].ToString());
                result.Add(getPlayerName(batsman));
                return result;

            }
            batsman = getPlayerId(message, a_order);
            result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["runs"].ToString());
            result.Add(getPlayerName(batsman));
            return result;


        }
        public JArray getPlayerFours(string message)
        {
            var obj = getData();
            JArray b_order = getBattingOrder("b");
            JArray a_order = (getBattingOrder("a"));
            JArray result = new JArray();
            string batsman = "";
            if (getPlayerId(message, a_order) == null)
            {
                if (getPlayerId(message, b_order) == null)
                {
                    result.Add(" You have spelt the player name wrong or " + Environment.NewLine + Environment.NewLine + " he/she is not a part of this match or " + Environment.NewLine + Environment.NewLine + " he/she havent played yet");
                    return result;
                }
                batsman = getPlayerId(message, b_order);

                result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["fours"].ToString());
                result.Add(getPlayerName(batsman));
                return result;

            }
            batsman = getPlayerId(message, a_order);
            result.Add(obj["card"]["players"][batsman]["match"]["innings"]["1"]["batting"]["fours"].ToString());
            result.Add(getPlayerName(batsman));
            return result;

        }

        public string getPlayerName(string name)
        {
            var obj = getData();
            return obj["card"]["players"][name]["fullname"].ToString();
        }
        public string getPlayerName() {
            var obj = getData();
            var name = (string)obj["card"]["now"]["striker"];
            return obj["card"]["players"][name]["fullname"].ToString();
        }
        public string getStrikerFours()
        {
            var obj = getData();
            var striker = (string)obj["card"]["now"]["striker"];
            return obj["card"]["players"][striker]["match"]["innings"]["1"]["batting"]["fours"].ToString();
        }

        public string getTeamFours()
        {
            var obj = getData();
            var batTeam = getBatTeam();
            var innings = getInnings();
            return obj["card"]["innings"][batTeam + "_" + innings]["fours"].ToString();
        }
        public string getBatTeam() {
            var obj = getData();
           return  (string)obj["card"]["now"]["batting_team"];
        }
        public string getInnings()
        {
            var obj = getData();
            return (string)obj["card"]["now"]["innings"];
        }
        public string getTeamSixes()
        {
            var obj = getData();
            var batTeam = getBatTeam();
            var innings = getInnings();
            return obj["card"]["innings"][batTeam + "_" + innings]["sixes"].ToString();
        }
        public string getStrikerSixes()
        {
            var obj = getData();
            var striker = (string)obj["card"]["now"]["striker"];
            return obj["card"]["players"][striker]["match"]["innings"]["1"]["batting"]["sixes"].ToString();
        }
        public string getStrikerDotBalls()
        {
            var obj = getData();
            var striker = (string)obj["card"]["now"]["striker"];
            return obj["card"]["players"][striker]["match"]["innings"]["1"]["batting"]["dotballs"].ToString();
        }
        public string getStrikerScore()
        {
            var obj = getData();
            var striker = (string)obj["card"]["now"]["striker"];
            return obj["card"]["players"][striker]["match"]["innings"]["1"]["batting"]["runs"].ToString();
        }
        public string getWhoWonTheMatch()
        {
            //Recent matches Api data access
            var obj = getData();
            if (getStatus().Equals("completed"))
                return obj["card"]["msgs"]["completed"].ToString();
            else
                return "match is going on";
        }
        public string getCurrentTeam() {
            var obj = getData();
            var batTeam = getBatTeam();
            return obj["card"]["teams"][batTeam]["name"].ToString();
        }
        public JArray getExtras()
        {
            var obj = getData();
            var batTeam = getBatTeam();
            var innings = getInnings();
            JArray extras = new JArray();
             extras.Add( obj["card"]["innings"][batTeam + "_" + innings]["extras"].ToString());
            extras.Add(obj["card"]["innings"][batTeam + "_" + innings]["legbye"].ToString());
            extras.Add(obj["card"]["innings"][batTeam + "_" + innings]["bye"].ToString());
            extras.Add(obj["card"]["innings"][batTeam + "_" + innings]["noball"].ToString());
            return extras;
        }
        public string getWicketsDown()
        {
            var obj = getData();
            var batTeam = getBatTeam();
            var innings = getInnings();
            return obj["card"]["innings"][batTeam + "_" + innings]["wickets"].ToString();
        }
        public string getMatchSummary()
        {
            var obj = getData();
            if (getStatus().Equals("completed"))
                return "the match completed with "+ getWhoWonTheMatch() + "as winners";
            else
                return obj["card"]["description"].ToString();
        }
        public JArray getCaptain() {
            var obj = getData();
            var a = (string)obj["card"]["teams"]["a"]["match"]["captain"];
            var aTeam =  (string)obj["card"]["teams"]["a"]["name"];
            var b = (string)obj["card"]["teams"]["b"]["match"]["captain"];
            var bTeam = (string)obj["card"]["teams"]["b"]["name"];
            var aCap = getPlayerName(a);
            var bCap = getPlayerName(b);
            JArray caps = new JArray();
            caps.Add("Captain of team " + aTeam + "is " + aCap);
            caps.Add("Captain of team " + bTeam + "is " + bCap);
            return caps;
        }
        public JArray getKeeper()
        {
            var obj = getData();
            var a = (string)obj["card"]["teams"]["a"]["match"]["keeper"];
            var aTeam = (string)obj["card"]["teams"]["a"]["name"];
            var b = (string)obj["card"]["teams"]["b"]["match"]["keeper"];
            var bTeam = (string)obj["card"]["teams"]["b"]["name"];
            var aKeep = getPlayerName(a);
            var bKeep = getPlayerName(b);
            JArray keeps = new JArray();
            keeps.Add("Keeper of team " + aTeam + "is " + aKeep);
            keeps.Add("Keeper of team " + bTeam + "is " + bKeep);
            return keeps;
        }
        public string getOversTillNow()
        {
            var obj = getData();
            var batTeam = getBatTeam();
            var innings = getInnings();
            return obj["card"]["innings"][batTeam + "_" + innings]["overs"].ToString();
        }
        public JArray getPatnership()
        {
            var obj = getData();
            var batTeam = getBatTeam();
            var innings = getInnings();
            JArray patnership =(JArray) obj["card"]["innings"][batTeam + "_" + innings]["partnerships"];
            int c = patnership.Count();
            JArray patners = new JArray();
            c = c - 1;
            
            patners.Add(getPlayerName(patnership[c]["player_b"].ToString()));
            patners.Add(getPlayerName(patnership[c]["player_a"].ToString()));
            patners.Add(patnership[c]["runs"].ToString());
            return patners;
        }
        
    }
}