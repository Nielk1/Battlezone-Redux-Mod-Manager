using CsQuery;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    class WorkshopContext
    {
        public static List<WorkshopMod> GetMods(int appid, string[] tags)
        {
            List<WorkshopMod> Mods = new List<WorkshopMod>();
            WebClient client = new WebClient();

            if (tags == null || tags.Length == 0)
                tags = new string[] { null };
            foreach (string tag in tags)
            {
                for (int page = 1; ; page++)
                {
                    string URL = $"https://steamcommunity.com/workshop/browse/?appid={appid}&browsesort=mostrecent&section=readytouseitems&actualsort=mostrecent";
                    if (!string.IsNullOrWhiteSpace(tag))
                        URL += $"&requiredtags%5B0%5D={tag}";
                    URL += $"&p={page}";

                    string rawDom = client.DownloadString(URL);
                    CQ dom = new CQ(rawDom);
                    CQ Items = dom[".workshopBrowseItems .workshopItem"];

                    for (int i = 0; i < Items.Length; i++)
                    {
                        CQ elem = new CQ(Items[i]);
                        string id = elem.Find(".ugc")?.Attr("data-publishedfileid");
                        string image = elem.Find(".workshopItemPreviewImage")?.Attr("src");
                        string title = elem.Find(".workshopItemTitle")?.Text();
                        CQ AuthorNode = elem.Find(".workshopItemAuthorName a");
                        string author = AuthorNode?.Text();
                        string author_url = AuthorNode?.Attr("href");
                        if (string.IsNullOrWhiteSpace(author_url))
                        {
                            author_url = null;
                        }
                        else
                        {
                            author_url = Path.GetDirectoryName(Path.GetDirectoryName(author_url));
                        }
                        Mods.Add(new WorkshopMod(image)
                        {
                            ID = id,
                            URL = $"https://steamcommunity.com/sharedfiles/filedetails/?id={id}",
                            Title = title,
                            Author = author,
                            AuthorUrl = author_url,
                        });
                    }

                    if (Items.Length != 30) // we are getting 30 per page, so if we have 0-29 we know we're done
                        break;

                    Thread.Sleep(1000);
                }
            }

            return Mods;
        }
    }

    public class WorkshopMod : ILinqListViewFindModsItem
    {
        public string IconKey => UniqueID;
        public string Name => Title;
        public string ModSource => "SteamCmd";
        public Image LargeIcon { get; private set; }
        public Image SmallIcon => null;
        public ListViewItem ListViewItemCache { get; set; }
        public string[] Tags { get; set; }


        public string UniqueID { get { return GetUniqueId(ID); } }
        public static string GetUniqueId(string workshopId) { return workshopId.PadLeft(UInt64.MaxValue.ToString().Length, '0') + "-SteamCmd"; }


        public bool Known
        {
            get
            {
                return !(Tags?.Contains("new") ?? true);
            }
            set
            {
                if (value)
                {
                    Tags = null;
                }
                else
                {
                    if (!(Tags?.Contains("new") ?? false))
                    {
                        Tags = new string[] { "new" };
                    }
                }
            }
        }



        public string URL { get; set; }
        public string ID { get; set; }
        public string Image { get; private set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string AuthorUrl { get; set; }

        public WorkshopMod(string Image)
        {
            this.Image = Image;
            try
            {
                WebRequest q = WebRequest.Create(Image);
                using (WebResponse r = q.GetResponse())
                {
                    LargeIcon = new Bitmap(r.GetResponseStream());
                }
            }
            catch { }
        }
    }
}
