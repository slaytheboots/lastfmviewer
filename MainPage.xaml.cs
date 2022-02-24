using System.Net.Http;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Foundation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace lastfmviewer
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Dialog();

        }

        private async void Dialog()
        {
            await DoSomethingEveryTenSeconds();
        }

        

        public async Task DoSomethingEveryTenSeconds()
        {
            while (true)
            {
                var delayTask = Task.Delay(1000);
                DoAThing();
                await delayTask; // wait until at least 10s elapsed since delayTask created
            }
        }

        private void DoAThing()
        {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                var resp = client.GetAsync("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=USERNAME&api_key=API_KEY&format=json");
                HttpContent requestContent = resp.Result.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                var data = JObject.Parse(jsonContent);
                var track = data["recenttracks"]["track"].ToString();
                JArray ro = JsonConvert.DeserializeObject<JArray>(track);
                int count = 1;
                foreach (JObject item in ro) // <-- Note that here we used JObject instead of usual JProperty
                {
                    if (count == 1)
                    {
                        string oldtitle = title.Text;
                        string artist = item.GetValue("artist").ToString();
                        JObject art = JsonConvert.DeserializeObject<JObject>(artist);
                        artisttt.Text = art.GetValue("#text").ToString();
                        string img = item.GetValue("image").ToString();
                        title.Text = item.GetValue("name").ToString();
                        // ...
                        count = 2;
                        JArray thing = JsonConvert.DeserializeObject<JArray>(img);
                        foreach (JObject itemm in thing)
                        {
                            string size = itemm.GetValue("size").ToString();
                            if (size == "extralarge")
                            {
                                string urlofthenewimage = itemm.GetValue("#text").ToString();
                                var oldimage = albumart.Source;
                                var newimage = new BitmapImage(new Uri(urlofthenewimage));
                                if (title.Text != oldtitle) {
                                albumart.Source = newimage;
                                albumart2.Source = newimage;
                                }
                            }
                        }

                    }
            }
        }
    }
}
