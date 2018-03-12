using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Crick_Bot;

namespace Crick_Bot
{
    [LuisModel("acdc813f-bdd9-41b9-9e20-48a109895794", "462a135179eb4b229bd05bb2e18faf9c")]
    [Serializable]
    public class Crick_Luis : LuisDialog<object>
    {
        private IEnumerable<string> matches_list = new List<string>();   // for matches list

        [LuisIntent("start_convo")]
        public async Task Start_convo(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hai");
            await context.PostAsync("I am Crick Bot " + Environment.NewLine + Environment.NewLine + "I can give you the details of Recent/Live cricket matches " + Environment.NewLine + Environment.NewLine + " and daily top news..");
 
            // displaying TopNews in Cards
            var reply = context.MakeMessage();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            await context.PostAsync("TODAY'S TOP NEWS..");
            reply.Attachments = Top_GetCardsAttachments();
            await context.PostAsync(reply);
            await RecentMatches_List(context);
        }
        public async virtual Task RecentMatches_List(IDialogContext context)
        {
            try
            {
                ResMethods rm = new ResMethods();
                matches_list = rm.getAllRecentList();
                // promt (list) displays the recent matches
                PromptDialog.Choice<string>(
                    context,
                    this.DisplayMatchesCard,
                    this.matches_list,
                    "Here are the Recent Matches :",
                    "Ooops, what you wrote is not a valid option, please try again",
                    3,
                    PromptStyle.PerLine);
            }
            catch (Exception e)
            {
                await context.PostAsync("error" + e.ToString());
            }
        }
        public async Task DisplayMatchesCard(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;
            await context.PostAsync($"{selectedCard.ToString()}");
            ResMethods rm = new ResMethods();
            string k = rm.getMatch_Key(selectedCard);
            if (k.Equals("none"))
                await context.PostAsync($"Can u select the match again.. ");
            else
            {
                Methods m = new Methods();
                m.setMatchKey(k.ToString());
                await context.PostAsync($"{selectedCard}U can ask anything about this match > ");
            }
        }

        [LuisIntent("about")]
        public async Task About(IDialogContext context, LuisResult result)
        {
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync("I am crick bot i can give you details of cricket matches and top news regarding the cricket matches..");
            context.Wait(MessageReceived);
        }
        [LuisIntent("who_won_toss")]
        public async Task Who_won_toss(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            string z = m.GetToss();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{z}");
            context.Wait(MessageReceived);
        }
        [LuisIntent("opening_players")]
        public async Task Opening_players(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            JArray openers = m.getOpeners();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{ openers[0].ToString() } and {openers[1].ToString()} are the opening players  and their patnership is {openers[2].ToString()}");
            context.Wait(MessageReceived);
        }
        [LuisIntent("current_batting_players")]
        public async Task Current_batting_players(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            if (m.getStatus().Equals("started"))
            {
                //get their scores and balls faced
                JArray currPlayers = m.getCurrPlayers();
                await context.PostAsync($"{currPlayers[0].ToString()} and {currPlayers[1].ToString()}are the players batting now.");
            }
            else if (m.getStatus().Equals("notstarted"))
                await context.PostAsync("As the match is not yet started !! There are no current players");
            else if (m.getStatus().Equals("completed"))
                await context.PostAsync("As the match is Completed !! There are no current players");
            context.Wait(MessageReceived);
        }
        [LuisIntent("current_bowling_player")]
        public async Task Current_bowling_players(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            if (m.getStatus().Equals("started"))
            {
                string bowler = m.getCurrBowler();
                Crick_Luis crick = new Crick_Luis();
                await context.PostAsync($"{bowler} is bowling now.");
            }
            else if (m.getStatus().Equals("notstarted"))
                await context.PostAsync("As the match is not yet started !! ");
            else if (m.getStatus().Equals("completed"))
                await context.PostAsync("As the match is Completed !! There is no current bowler");
            context.Wait(MessageReceived);
        }
        [LuisIntent("highest_scored_player_batting")]
        public async Task highest_scored_player_batting(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            if (m.getStatus().Equals("started"))
            {
                var team = m.getBatTeam();
                var res = m.getHighestBatsman(team);
                await context.PostAsync($"{res.ToString()}");
            }
            else if (m.getStatus().Equals("notstarted"))
            {
                await context.PostAsync("The match is not yet Started");
            }
            else if (m.getStatus().Equals("completed"))
            {
                var aRes = m.getHighestBatsman("a");
                var bRes = m.getHighestBatsman("b");
                await context.PostAsync($"{aRes.ToString()} {Environment.NewLine + Environment.NewLine} {bRes.ToString()}");
            }

            context.Wait(MessageReceived);
        }
        [LuisIntent("highest_wicket_drawn_bowler")]
        public async Task highest_wickets_drawn_bowler(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            if (m.getStatus().Equals("started"))
            {
                var team = m.getBowlTeam();
                var res = m.getHighWBowler(team);
                await context.PostAsync($"{res.ToString()}");
            }
            else if (m.getStatus().Equals("notstarted"))
            {
                await context.PostAsync("The match is not yet Started");
            }
            else if (m.getStatus().Equals("completed"))
            {
                var aRes = m.getHighWBowler("a");
                var bRes = m.getHighWBowler("b");
                await context.PostAsync($"{aRes.ToString()} {Environment.NewLine + Environment.NewLine} {bRes.ToString()}");
            }
            else await context.PostAsync(" No wickets fallen yet!!!");

            context.Wait(MessageReceived);
        }
        [LuisIntent("man_of_the_match")]
        public async Task Man_of_the_match(IDialogContext context, LuisResult result)
        {

            Methods m = new Methods();
            string man = m.getManOfTheMatch();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{man.ToString()}");
            context.Wait(MessageReceived);
        }
        [LuisIntent("match_summary")]
        public async Task Match_summary(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            string summary = m.getMatchSummary();
            await context.PostAsync($" {summary.ToString()} ");
            context.Wait(MessageReceived);
        }
        [LuisIntent("place_of_match")]
        public async Task Place_of_match(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            string z = m.getMatchVenue();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{z} is the venue");
            context.Wait(MessageReceived);
        }
        [LuisIntent("batsman_strike_rate")]
        public async Task Batsman_strike_rate(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($" in luis {result.Query.ToString()}");
            if (m.getStatus().Equals("notstarted"))
            {
                await context.PostAsync("Strike rate can only be displayed for live matches");

            }
            else
            {
                JArray strike_rate = m.getStrikeRate(result.Query.ToString());
                if (strike_rate.Count() > 1)
                {
                    await context.PostAsync($" {strike_rate[0].ToString()} is the strike rate of  {strike_rate[1].ToString()}");
                }
                else
                {
                    await context.PostAsync($" {strike_rate[0].ToString()}");
                }
            }
            context.Wait(MessageReceived);

        }
        [LuisIntent("batsman_score")]
        public async Task Batsman_score(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($" in luis {result.Query.ToString()}");
            if (m.getStatus().Equals("notstarted"))
            {
                await context.PostAsync("Score can only be displayed for live matches");

            }
            else
            {
                JArray strike_rate = m.getPlayerScore(result.Query.ToString());
                if (strike_rate.Count() > 1)
                {
                    await context.PostAsync($" {strike_rate[0].ToString()} is the score of  {strike_rate[1].ToString()}");
                }
                else
                {
                    await context.PostAsync($" {strike_rate[0].ToString()}");
                }
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("who_won_match")]
        public async Task Who_won_match(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            string matchResult = m.getWhoWonTheMatch();

            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{matchResult.ToString()}");
            context.Wait(MessageReceived);
        }
        [LuisIntent("no.of_six_player")]
        public async Task No_of_six_player(IDialogContext context, LuisResult result)
        {

            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($" in luis {result.Query.ToString()}");
            if (m.getStatus().Equals("notstarted"))
            {
                await context.PostAsync("score can only be displayed for live matches");

            }
            else
            {
                JArray strike_rate = m.getPlayerSixes(result.Query.ToString());
                if (strike_rate.Count() > 1)
                {
                    await context.PostAsync($" {strike_rate[0].ToString()} are the sixes scored by  {strike_rate[1].ToString()}");
                }
                else
                {
                    await context.PostAsync($" {strike_rate[0].ToString()}");
                }
            }
            context.Wait(MessageReceived);
        }
        [LuisIntent("no.of_six_team")]
        public async Task No_of_six_team(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            if (m.getStatus().Equals("started"))
            {
                string six_score = m.getTeamSixes();
                string team = m.getCurrentTeam();

                await context.PostAsync($" {team.ToString()} scored  {six_score.ToString()} sixes till now");
            }
            else
            {
                await context.PostAsync("No live data to display");
            }
            context.Wait(MessageReceived);
        }
        [LuisIntent("no.of_four_player")]
        public async Task No_of_four_player(IDialogContext context, LuisResult result)
        {

            Methods m = new Methods();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($" in luis {result.Query.ToString()}");
            if (m.getStatus().Equals("notstarted"))
            {
                await context.PostAsync("Strike rate can only be displayed for live matches");

            }
            else
            {
                JArray strike_rate = m.getPlayerFours(result.Query.ToString());
                if (strike_rate.Count() > 1)
                {
                    await context.PostAsync($" {strike_rate[0].ToString()} fours  are scored by {strike_rate[1].ToString()}");
                }
                else
                {
                    await context.PostAsync($" {strike_rate[0].ToString()}");
                }
            }
            context.Wait(MessageReceived);
        }
        [LuisIntent("no.of_four_team")]
        public async Task No_of_four_team(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            string four_score = m.getTeamFours();
            string team = m.getCurrentTeam();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($" {team.ToString()} scored  {four_score.ToString()} fours till now ");
            context.Wait(MessageReceived);
        }
        [LuisIntent("final_team_score")]
        public async Task Final_score(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            string score = m.getCurrScore();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{score.ToString()} is the total score");
            context.Wait(MessageReceived);
        }
        [LuisIntent("wickets_down")]
        public async Task Wickets_down(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            string wickets = m.getWicketsDown();
            string team = m.getCurrentTeam();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{team.ToString()} has  lost {wickets.ToString()} wickets");
            context.Wait(MessageReceived);
        }
        [LuisIntent("extras")]
        public async Task Extras(IDialogContext context, LuisResult result)
        {

            Methods m = new Methods();
            JArray extras = m.getExtras();
            string team = m.getCurrentTeam();
            Crick_Luis crick = new Crick_Luis();
            List<string> extrasList = new List<string> { };
            extrasList.Add("Extras   : " + extras[0].ToString());
            extrasList.Add("Legbye   : " + extras[1].ToString());
            extrasList.Add("Bye      : " + extras[2].ToString());
            extrasList.Add("No Balls : " + extras[3].ToString());
            await context.PostAsync($"{team.ToString()} {Environment.NewLine + Environment.NewLine} ");
            await context.PostAsync($"{extrasList[0].ToString()} {Environment.NewLine + Environment.NewLine} {extrasList[1].ToString()}{Environment.NewLine + Environment.NewLine} {extrasList[2].ToString()}{Environment.NewLine + Environment.NewLine} {extrasList[3].ToString()}");
            context.Wait(MessageReceived);
        }
        [LuisIntent("completed_overs")]
        public async Task completed_overs(IDialogContext context, LuisResult result)
        {

            Methods m = new Methods();
            string extras = m.getOversTillNow();
            string team = m.getCurrentTeam();
            Crick_Luis crick = new Crick_Luis();

            await context.PostAsync($"{team.ToString()} has  completed {extras.ToString()} overs till now ");
            context.Wait(MessageReceived);
        }
        [LuisIntent("current_players_partnership")]
        public async Task partnership(IDialogContext context, LuisResult result)
        {

            Methods m = new Methods();

            JArray partners = m.getPatnership();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{  partners[0].ToString() } Vs { partners[1].ToString()}  and their patnership is { partners[2].ToString()}");
            context.Wait(MessageReceived);
        }
        [LuisIntent("captain")]
        public async Task captains(IDialogContext context, LuisResult result)
        {

            Methods m = new Methods();
            JArray caps = m.getCaptain();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{  caps[0].ToString()} {Environment.NewLine + Environment.NewLine}{ caps[1].ToString()}");
            context.Wait(MessageReceived);
        }
        [LuisIntent("wicket_keeper")]
        public async Task wicket_keeper(IDialogContext context, LuisResult result)
        {
            Methods m = new Methods();
            JArray keeps = m.getKeeper();
            Crick_Luis crick = new Crick_Luis();
            await context.PostAsync($"{  keeps[0].ToString()} {Environment.NewLine + Environment.NewLine}{ keeps[1].ToString()}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("end_convo")]
        public async Task End_convo(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("bye!!");
            context.Wait(MessageReceived);
        }
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry!!" + Environment.NewLine + Environment.NewLine + "I did'nt get you..." + Environment.NewLine + Environment.NewLine + "Try asking me different queries..");
            context.Wait(MessageReceived);
        }
        public class Article
        {
            public string author { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public string urlToImage { get; set; }
            public string publishedAt { get; set; }
        }
        private class RootObject
        {
            public string status { get; set; }
            public string source { get; set; }
            public string sortBy { get; set; }
            public List<Article> articles { get; set; }
        }
        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };
            return heroCard.ToAttachment();
        }
        private static IList<Attachment> Top_GetCardsAttachments()
        {
            HttpReqRes res = new HttpReqRes();
            string EndPoint = "https://newsapi.org/v1/articles?source=espn-cric-info&sortBy=top&apiKey=70ecac24214846759b5ca1eb23c25329";
            string res_topnews = res.MakeRequest(EndPoint);
            RootObject ro = JsonConvert.DeserializeObject<RootObject>(res_topnews);
            String auth = "Author" + ro.articles[0].author;
            return new List<Attachment>()
            { 
                GetHeroCard(
                      ro.articles[0].title,
                      auth,
                      ro.articles[0].description,
                     new CardImage(url: ro.articles[0].urlToImage),
                     new CardAction(ActionTypes.OpenUrl, "Learn more", value:ro.articles[0].url)),
             GetHeroCard(
                     ro.articles[1].title,
                     auth,
                     ro.articles[1].description,
                    new CardImage(url: ro.articles[1].urlToImage),
                    new CardAction(ActionTypes.OpenUrl, "Learn more", value: ro.articles[1].url)),
             GetHeroCard(
                      ro.articles[2].title,
                      auth,
                      ro.articles[2].description,
                     new CardImage(url: ro.articles[2].urlToImage),
                     new CardAction(ActionTypes.OpenUrl, "Learn more", value:ro.articles[2].url)),
             GetHeroCard(
                      ro.articles[3].title,
                      auth,
                      ro.articles[3].description,
                     new CardImage(url: ro.articles[3].urlToImage),
                     new CardAction(ActionTypes.OpenUrl, "Learn more", value:ro.articles[3].url)),
             GetHeroCard(
                      ro.articles[4].title,
                      auth,
                      ro.articles[4].description,
                     new CardImage(url: ro.articles[4].urlToImage),
                     new CardAction(ActionTypes.OpenUrl, "Learn more", value:ro.articles[4].url)),
                 };
        }
    }
}