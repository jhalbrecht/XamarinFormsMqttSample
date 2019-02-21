using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RepoDates.ConsoleApp
{
    // created_at in this will give you github repo create date https://api.github.com/repos/jhalbrecht/XamarinFormsMqttSample

    class Program
    {
        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }

        static async Task MainAsync(string[] args)
        {
            /*
                Why did I write this? I just needed some encouragment by completing a task in visual studio.
                I'd been working on trying to learn how to TLS secure a XamarinForms app for 24 days.
                I was getting tired, cranky, frustrated for not being able to find just the right docs to solve it myself ....

                Now in case I want to complain, moan or wavie my hands around for perhaps a "There, there", but
                probably a "RTFM bunky!" on twitter I could quickly calculate how long I've been working on this! :-)
            
                This code will display create date and age of a GitHub repository.
                change these to your liking;
                string repoOwner = "jhalbrecht";                Your/any GitHub account.
                string repo = "XamarinFormsMqttSample";         Your/any Repository.
            */

            Console.WriteLine("Hello RepoDates World!");

            string repoOwner = "jhalbrecht";
            string repo = "XamarinFormsMqttSample";

            string baseurl = "https://api.github.com/repos";
            string rawjsonUrl = Path.Combine(baseurl, repoOwner, repo);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Anything");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(rawjsonUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            client.Dispose();

            RepoInfo repoInfo = JsonConvert.DeserializeObject<RepoInfo>(responseBody);
            DateTime createdDate = repoInfo.created_at;
            TimeSpan repoDaysOld = DateTime.Now - createdDate;

            Console.WriteLine($"The current Date and Time is; {DateTime.Now}\n");
            Console.WriteLine($"GitHub Repo {repo}, created by {repoOwner} on; {createdDate}. It is {repoDaysOld.ToString("dd")} days old.");
            Console.WriteLine("\npress any key to terminate");
            Console.ReadKey();
        }

        // good 'ol http://json2csharp.com/ ToDo: get the create date without having these classes?
        public class RepoOwner
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class RepoInfo
        {
            public int id { get; set; }
            public string node_id { get; set; }
            public string name { get; set; }
            public string full_name { get; set; }
            public bool @private { get; set; }
            public RepoOwner owner { get; set; }
            public string html_url { get; set; }
            public string description { get; set; }
            public bool fork { get; set; }
            public string url { get; set; }
            public string forks_url { get; set; }
            public string keys_url { get; set; }
            public string collaborators_url { get; set; }
            public string teams_url { get; set; }
            public string hooks_url { get; set; }
            public string issue_events_url { get; set; }
            public string events_url { get; set; }
            public string assignees_url { get; set; }
            public string branches_url { get; set; }
            public string tags_url { get; set; }
            public string blobs_url { get; set; }
            public string git_tags_url { get; set; }
            public string git_refs_url { get; set; }
            public string trees_url { get; set; }
            public string statuses_url { get; set; }
            public string languages_url { get; set; }
            public string stargazers_url { get; set; }
            public string contributors_url { get; set; }
            public string subscribers_url { get; set; }
            public string subscription_url { get; set; }
            public string commits_url { get; set; }
            public string git_commits_url { get; set; }
            public string comments_url { get; set; }
            public string issue_comment_url { get; set; }
            public string contents_url { get; set; }
            public string compare_url { get; set; }
            public string merges_url { get; set; }
            public string archive_url { get; set; }
            public string downloads_url { get; set; }
            public string issues_url { get; set; }
            public string pulls_url { get; set; }
            public string milestones_url { get; set; }
            public string notifications_url { get; set; }
            public string labels_url { get; set; }
            public string releases_url { get; set; }
            public string deployments_url { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public DateTime pushed_at { get; set; }
            public string git_url { get; set; }
            public string ssh_url { get; set; }
            public string clone_url { get; set; }
            public string svn_url { get; set; }
            public string homepage { get; set; }
            public int size { get; set; }
            public int stargazers_count { get; set; }
            public int watchers_count { get; set; }
            public string language { get; set; }
            public bool has_issues { get; set; }
            public bool has_projects { get; set; }
            public bool has_downloads { get; set; }
            public bool has_wiki { get; set; }
            public bool has_pages { get; set; }
            public int forks_count { get; set; }
            public object mirror_url { get; set; }
            public bool archived { get; set; }
            public int open_issues_count { get; set; }
            public object license { get; set; }
            public int forks { get; set; }
            public int open_issues { get; set; }
            public int watchers { get; set; }
            public string default_branch { get; set; }
            public int network_count { get; set; }
            public int subscribers_count { get; set; }
        }
    }
}

// I want to remember this enum usage...
//public enum TimeComparison
//{
//    EarlierThan = -1,
//    TheSameAs = 0,
//    LaterThan = 1
//}

//DateTimeOffset thisDate2 = new DateTimeOffset(2011, 6, 10, 15, 24, 16,
//                                              TimeSpan.Zero);
//Console.WriteLine("The current date and time: {0:MM/dd/yy H:mm:ss zzz}",
//                   thisDate2);



//DateTime localTime = DateTime.Now;
//DateTime utcTime = DateTime.UtcNow;

//Console.WriteLine("Difference between {0} and {1} time: {2}:{3} hours",
//                  localTime.Kind.ToString(),
//                  utcTime.Kind.ToString(),
//                  (localTime - utcTime).Hours,
//                  (localTime - utcTime).Minutes);
//Console.WriteLine("The {0} time is {1} the {2} time.",
//                  localTime.Kind.ToString(),
//                  Enum.GetName(typeof(TimeComparison), localTime.CompareTo(utcTime)),
//                  utcTime.Kind.ToString());
