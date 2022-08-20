using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Transform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            JArray transformed = new JArray();
            var newList = "https://bzamayo.com/extras/apple-tv-screensavers.json";
            var phobosURL = "http://a1.phobos.apple.com/us/r1000/000/Features/atv/AutumnResources/videos/entries.json";

            var webClient = new WebClient();

            var textNewList = webClient.DownloadString(newList);
            var textOldList = webClient.DownloadString(phobosURL);

            var jsonNew = JObject.Parse(textNewList);
            var jsonOld = JArray.Parse(textOldList);

            var assets = new JArray();
            var wrapper = new JObject();
            var item = new JProperty("id", Guid.NewGuid().ToString());

            wrapper.AddFirst(item);

            var index = 0;
            foreach (var oldData in jsonOld)
            {
                foreach (var ss in oldData["assets"])
                {
                    var asset = new JObject();

                    asset.Add(new JProperty("url", ss["url"].ToString().Replace("https://", "http://")));
                    asset.Add(new JProperty("accessibilityLabel", ss["accessibilityLabel"]));                    
                    asset.Add(new JProperty("timeOfDay", ss["timeOfDay"]));
                    asset.Add(new JProperty("type", ss["type"]));
                    //asset.Add(new JProperty("id", ss["id"]));
                    asset.Add(new JProperty("id", index.ToString()));

                    assets.Add(asset);
                    index++;
                }
            }

            var dataNew = (JArray)jsonNew["data"];
            foreach (var el in dataNew)
            {
                var name = el["name"];
                var screensavers = (JArray)el["screensavers"];

                foreach (var ss in screensavers)
                {
                    var asset = new JObject();
                    asset.Add(new JProperty("url", ss["videoURL"].ToString().Replace("https://", "http://")));
                    asset.Add(new JProperty("accessibilityLabel", name));
                    //asset.Add(new JProperty("id", ss["identifier"]));
                    asset.Add(new JProperty("id", index.ToString()));
                    asset.Add(new JProperty("timeOfDay", "unknown"));
                    asset.Add(new JProperty("type", "video"));

                    assets.Add(asset);
                    index++;
                }
            }

            wrapper.Add(new JProperty("assets", assets));

            transformed.Add(wrapper);
            var fin = transformed.ToString(Formatting.Indented);
            Trace.WriteLine(fin);

            File.WriteAllText(@"d:\test.json", transformed.ToString(Formatting.Indented));
        }
    }
}
