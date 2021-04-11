using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack;
using startupTweetMiner.Models;

namespace startupTweetMiner
{
    class TweetMiner
    {
        static bool stopProgram = false;
        static HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            while (!stopProgram)
            {
                stopProgram = await RunProgram();
            }
            Console.WriteLine("Adios! Hit enter to exit");
            Console.Read();
        }

        static async Task<bool> RunProgram()
        {
            Console.WriteLine("Let's pull some tweets!");
            Console.Write("Enter start date to search tweets: ");
            string startDate = Console.ReadLine();

            Console.Write("Enter end date to search tweets: ");
            string endDate = Console.ReadLine();

            List<Entity> danishStartups = new List<Entity>
            {
                new Entity{ Handle = "NordicAPI", Location = "Denmark", AuthorId = "1039131378250657792"},
                new Entity{ Handle = "raffleAI", Location = "Denmark", AuthorId = "1070593629377433601"},
                new Entity{ Handle = "cerebriu",Location = "Denmark",AuthorId = "1205464370081386496"},
                new Entity{ Handle = "GamerzClass",Location = "Denmark", AuthorId = "1006960874538618880"},
                new Entity{ Handle = "DreamdataIO",Location = "Denmark", AuthorId = "1123507082156806145"},
                new Entity{ Handle = "pentohq",Location = "Denmark",AuthorId = "791984262396403713"},
                new Entity{ Handle = "SeaborgTech",Location = "Denmark",AuthorId = "2794696790"},
                new Entity{ Handle = "Canopy_LAB",Location = "Denmark",AuthorId = "2976507786"},
                new Entity{ Handle = "MakerDAO",Location = "Denmark",AuthorId = "3190865591"},
                new Entity{ Handle = "TheBrainPlus",Location = "Denmark",AuthorId = "1481740458"},
                new Entity{ Handle = "eloomiGO",Location = "Denmark",AuthorId = "3110818047"},
                new Entity{ Handle = "airtame",Location = "Denmark",AuthorId = "1290890174"},
                new Entity{ Handle = "happyhelperdk",Location = "Denmark",AuthorId = "3342639514"},
                new Entity{ Handle = "GetLinkfire",Location = "Denmark",AuthorId = "3342639514"},
                new Entity{ Handle = "Orphazyme_AS",Location = "Denmark",AuthorId = "3216396538"},
                new Entity{ Handle = "Sleeknotecom",Location = "Denmark",AuthorId = "1938401425"},
                new Entity{ Handle = "wearehiveonline",Location = "Denmark",AuthorId = "807653240183484416"},
                new Entity{ Handle = "SepiorCorp",Location = "Denmark",AuthorId = "1693850071"},
                new Entity{ Handle = "ARYZEofficial",Location = "Denmark",AuthorId = "954372098687930368"},
                new Entity{ Handle = "graduateland",Location = "Denmark",AuthorId = "121728016"},
                new Entity{ Handle = "famlyhq",Location = "Denmark",AuthorId = "1677601039"},
                new Entity{ Handle = "nordinvestments",Location = "Denmark",AuthorId = "3315035303"},
                new Entity{ Handle = "mofibo",Location = "Denmark",AuthorId = "1407138194"},
                new Entity{ Handle = "PlantJammer",Location = "Denmark",AuthorId = "811948965730156544"},
                new Entity{ Handle = "Vestas",Location = "Denmark",AuthorId = "156646851"}
            };
            List<Entity> bostonStartups = new List<Entity> {
                new Entity{ Handle = "dayzerodx", Location = "Boston", AuthorId="761492744435032064"},
                new Entity{ Handle = "enginexyz", Location = "Boston", AuthorId="790711822697504768"},
                new Entity{ Handle = "careacademyco", Location = "Boston", AuthorId="407234316"},
                new Entity{ Handle = "EnkoChem", Location = "Boston", AuthorId="1159130140032602114"},
                new Entity{ Handle = "CauseEDU", Location = "Boston", AuthorId="924268464591769600"},
                new Entity{ Handle = "tiledb", Location = "Boston", AuthorId="913862119531085827"},
                new Entity{ Handle = "LuminDx", Location = "Boston", AuthorId="1010187111067279361"},
                new Entity{ Handle = "our_foodspace", Location = "Boston", AuthorId="831293738731315200"},
                new Entity{ Handle = "liteboxer", Location = "Boston", AuthorId="1093945337939705857"},
                new Entity{ Handle = "somerville_ev", Location = "Boston", AuthorId="1060689480250736640"},
                new Entity{ Handle = "livecobu", Location = "Boston", AuthorId="4867945113"},
                new Entity{ Handle = "bloomertech", Location = "Boston", AuthorId="891478379156963328"},
                new Entity { Handle = "flarejewelry", Location = "Boston", AuthorId="1173211160654561281"},
                new Entity{ Handle = "hometap", Location = "Boston", AuthorId="968548867066253314"},
                new Entity{ Handle = "neuralmagic", Location = "Boston", AuthorId="997536616481722369"},
                new Entity{ Handle = "NeighborSchools", Location = "Boston", AuthorId="1035931660397760512"},
                new Entity{ Handle = "starburstdata", Location = "Boston", AuthorId="925124076666007558"},
                new Entity{ Handle = "HiMarleyInc", Location = "Boston", AuthorId="1118930213449207808"},
                new Entity{ Handle = "thrasio", Location = "Boston", AuthorId="1093889859251564544"},
                new Entity{ Handle = "fritzlabs", Location = "Boston", AuthorId="929081759312031744"},
                new Entity{ Handle = "blink_ai", Location = "Boston", AuthorId="932674158869270529"},
                new Entity{ Handle = "AliroQuantum", Location = "Boston", AuthorId="1173193724903460865"},
                new Entity{ Handle = "joindough", Location = "Boston", AuthorId="1136363591186157568"},
                new Entity{ Handle = "Fairmarkit", Location = "Boston", AuthorId="830427237124026368"},
                new Entity { Handle = "flipsidecrypto", Location = "Boston", AuthorId="925712018937712640"},
            };

            Console.Write("Do you want to gather tweets or the mentions? (T/M): ");
            string tweetOrMention = Console.ReadLine().ToLower();
            string fileNameAppend = tweetOrMention.Equals("m") ? "StartUpMentions" : "StartUpTweets";

            Console.WriteLine("Gathering tweets....");
            List<Tweet> tweets = await GatherTweets(startDate, endDate, danishStartups, bostonStartups, tweetOrMention);
            List<Tweet> danishTweets = tweets.Where(x => danishStartups.Any(y => y.Handle.Equals(x.Handle))).ToList();
            List<Tweet> bostonTweets = tweets.Where(x => bostonStartups.Any(y => y.Handle.Equals(x.Handle))).ToList();

            Console.WriteLine($"Done! {tweets.Count} total tweets found between {startDate}-{endDate}");
            Console.WriteLine($"Danish tweets: {danishTweets.Count}");
            Console.WriteLine($"Boston tweets: {bostonTweets.Count}");
            Console.Write("Do you want to try a different timeframe? (Y/N) ");

            if (Console.ReadLine().ToLower().Equals("y")) return false;

            Console.WriteLine("Writing data file to desktop...");
            string filePath = WriteToFile(tweets, startDate, endDate, fileNameAppend);
            Console.WriteLine($"File saved to: {filePath}");

            return true;
        }

        private static async Task<List<Tweet>> GatherTweets(string startDate, string endDate, List<Entity> danishStartupsHandles, List<Entity> bostonStartupHandles, string tweetType)
        {
            List<Entity> combinedHandles = danishStartupsHandles.Concat(bostonStartupHandles).ToList();
            string formattedStartDate = DateTime.Parse(startDate).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            string formattedEndDate = DateTime.Parse(endDate).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            ConcurrentQueue<Tweet> minedTweets = new ConcurrentQueue<Tweet>();

            foreach (var entity in combinedHandles)
            {
                minedTweets = await GatherAndParse(entity, tweetType, formattedStartDate, formattedEndDate, minedTweets);
            }
            return minedTweets.ToList();
        }

        static async Task<ConcurrentQueue<Tweet>> GatherAndParse(Entity entity, string tweetType, string formattedStartDate, string formattedEndDate, ConcurrentQueue<Tweet> minedTweets)
        {
            string bearerToken = "";
            string nextBatchToken = "";
            bool firstRun = true;

            while (firstRun || !nextBatchToken.IsNullOrEmpty())
            {
                Thread.Sleep(1000);
                string searchUrl = "";

                //first mention run without pagination
                if (tweetType.Equals("m") && firstRun)
                {
                    searchUrl = $"https://api.twitter.com/2/users/{entity.AuthorId}/mentions?max_results=100&start_time={formattedStartDate}&end_time={formattedEndDate}&tweet.fields=created_at,author_id,referenced_tweets,attachments,entities,public_metrics,lang";
                }
                //following mention runs with pagination
                else if (tweetType.Equals("m") && !nextBatchToken.IsNullOrEmpty())
                {
                    Console.WriteLine($"Getting next batch of mentions for {entity.Handle}...");
                    searchUrl = $"https://api.twitter.com/2/users/{entity.AuthorId}/mentions?max_results=100&start_time={formattedStartDate}&end_time={formattedEndDate}&tweet.fields=created_at,author_id,referenced_tweets,attachments,entities,public_metrics,lang&pagination_token={nextBatchToken}";
                }
                // first tweet run without pagination
                else if (tweetType.Equals("t") && firstRun)
                {
                    searchUrl = $"https://api.twitter.com/2/tweets/search/all?query=from:{entity.Handle}&max_results=500&start_time={formattedStartDate}&end_time={formattedEndDate}&tweet.fields=created_at,author_id,referenced_tweets,attachments,entities,public_metrics,lang";
                }
                //following tweet runs with pagination
                else
                {
                    Console.WriteLine($"Getting next batch of tweets for {entity.Handle}...");
                    searchUrl = $"https://api.twitter.com/2/tweets/search/all?query=from:{entity.Handle}&max_results=500&start_time={formattedStartDate}&end_time={formattedEndDate}&tweet.fields=created_at,author_id,referenced_tweets,attachments,entities,public_metrics,lang&pagination_token={nextBatchToken}";
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                var responseMessage = await httpClient.GetAsync(searchUrl);
                if (int.Parse(responseMessage.Headers.GetValues("x-rate-limit-remaining").FirstOrDefault()) == 0)
                {
                    Console.WriteLine("Rate limit reached, sleeping for 15 minutes...");
                    Thread.Sleep(900000);
                    Console.WriteLine("Resuming!");
                }

                var jsonToObject = new List<Dictionary<string, object>>();
                var jsonMetaData = new Dictionary<string, object>();
                using (HttpContent content = responseMessage.Content)
                {
                    var json = content.ReadAsStringAsync().Result;
                    var jObject = JObject.Parse(json);

                    if (!jObject.ContainsKey("data"))
                    {
                        Console.WriteLine($"No tweets found for {entity.Handle}");
                        break;
                    }
                    jsonToObject = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jObject["data"].ToString());
                    jsonMetaData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jObject["meta"].ToString());
                }

                int tweetCounter = 0;
                Parallel.ForEach(jsonToObject, tweetJson =>
                {
                    JObject publicMetricsJobj = (JObject)tweetJson.GetValueOrDefault("public_metrics");
                    Dictionary<string, int> publicMetrics = JsonConvert.DeserializeObject<Dictionary<string, int>>(publicMetricsJobj.ToString());

                    List<Url> mediaData = new List<Url>();
                    JObject entitiesJobj = (JObject)tweetJson.GetValueOrDefault("entities");
                    if (entitiesJobj is not null)
                    {
                        Dictionary<string, object> entities = JsonConvert.DeserializeObject<Dictionary<string, object>>(entitiesJobj.ToString());
                        JArray urlJobj = (JArray)entities.GetValueOrDefault("urls");
                        if (urlJobj is not null) mediaData = urlJobj.ToObject<List<Url>>();
                    }

                    JArray referenceTweetJobj = (JArray)tweetJson.GetValueOrDefault("referenced_tweets");
                    List<ReferencedTweet> referencedTweets = referenceTweetJobj is null ? new List<ReferencedTweet>() : referenceTweetJobj.ToObject<List<ReferencedTweet>>();

                    minedTweets.Enqueue(new Tweet
                    {
                        Handle = entity.Handle,
                        Location = entity.Location,
                        AuthorId = (string)tweetJson.GetValueOrDefault("author_id"),
                        TweetId = (string)tweetJson.GetValueOrDefault("id"),
                        Text = ((string)tweetJson.GetValueOrDefault("text")).Replace("'", ""),
                        CreatedAt = ((DateTime)tweetJson.GetValueOrDefault("created_at")).ToString(),
                        Language = (string)tweetJson.GetValueOrDefault("lang"),
                        MediaData = mediaData,
                        ReferencedTweets = referencedTweets,
                        RetweetCount = publicMetrics.GetValueOrDefault("retweet_count"),
                        ReplyCount = publicMetrics.GetValueOrDefault("reply_count"),
                        LikeCount = publicMetrics.GetValueOrDefault("like_count"),
                        QuoteCount = publicMetrics.GetValueOrDefault("quote_count"),
                    });

                    tweetCounter++;
                });

                // get meta data and check for next token
                if (jsonMetaData.GetValueOrDefault("next_token") is not null) nextBatchToken = (string)jsonMetaData.GetValueOrDefault("next_token");
                else nextBatchToken = "";

                firstRun = false;

                Console.WriteLine($"{tweetCounter} tweets stored for {entity.Handle}");
                tweetCounter = 0;
            }
            return minedTweets;
        }

        private static string WriteToFile(List<Tweet> tweets, string startDate, string endDate, string dataType)
        {
            var csvFilePath = $"/Users/{Environment.UserName}/Desktop/{dataType}_{startDate.ReplaceAll("/","-")}-{endDate.ReplaceAll("/", "-")}.csv".MapAbsolutePath();

            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<Tweet>();
                csv.NextRecord();
                foreach (var tweet in tweets)
                {
                    Tweet extendedTweet = tweet;
                    extendedTweet.MediaData.ForEach(media =>
                    {
                        extendedTweet.MediaDataUrl = media.url;
                        extendedTweet.MediaTitle = media.title;
                        extendedTweet.MediaDescription = media.description;
                    });
                    extendedTweet.ReferencedTweets.ForEach(reference =>
                    {
                        extendedTweet.ReferencedTweetId = reference.Id;
                        extendedTweet.ReferencedTweetType = reference.Type;
                    });

                    csv.WriteRecord(extendedTweet);
                    csv.NextRecord();
                }
            }

            return csvFilePath;
        }
    }
}
