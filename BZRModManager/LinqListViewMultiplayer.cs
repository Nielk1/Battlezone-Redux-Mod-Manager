using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace BZRModManager
{
    class LinqListViewMultiplayer : System.Windows.Forms.ListView
    {
        public LinqListViewMultiplayer()
        {
            // This call is required by the Windows.Forms Form Designer.
            //InitializeComponent();

            DataSource = null;

            base.RetrieveVirtualItem += LinqListView_RetrieveVirtualItem;
        }

        private void LinqListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                ListViewItem item = this.GetItemAt(5, e.Y);
                if (item == null) return;
            }
        }

        private void LinqListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //ILinqListView2Item item = source[e.ItemIndex];
            LinqListViewMultiplayerItem item = source.ElementAt(e.ItemIndex);

            if (item.ListViewItemCache != null)
            {
                e.Item = item.ListViewItemCache;
                return;
            }

            if (item.LargeIcon != null)
            {
                if (!LargeImageList.Images.ContainsKey(item.IconKey))
                {
                    LargeImageList.Images.Add(item.IconKey, item.LargeIcon);
                }
            }
            if (item.SmallIcon != null)
            {
                if (!SmallImageList.Images.ContainsKey(item.IconKey))
                {
                    SmallImageList.Images.Add(item.IconKey, item.SmallIcon);
                }
            }

            ListViewItem lvi = new ListViewItem(item.Name, LargeImageList.Images.IndexOfKey(item.IconKey));
            lvi.UseItemStyleForSubItems = false;
            lvi.Tag = item;
            lvi.SubItems.Add(item.PlayerCount);
            lvi.SubItems.Add(item.GameType);
            lvi.SubItems.Add(item.GameMode);
            lvi.SubItems.Add(item.Map);
            if(!string.IsNullOrWhiteSpace(item.Mod))
            {
                if(internal_source.Mods.ContainsKey(item.Mod))
                {
                    lvi.SubItems.Add(internal_source.Mods[item.Mod].Name);
                }
                else
                {
                    lvi.SubItems.Add(item.Mod);
                }
            }
            else
            {
                lvi.SubItems.Add("Stock");
            }
            lvi.SubItems.Add(item.MotD);
            e.Item = lvi;

            item.ListViewItemCache = lvi;
        }

        private List<LinqListViewMultiplayerItem> source;
        private MultiplayerGamelistData internal_source;

        public LinqListViewMultiplayerItem GetItemAtVirtualIndex(int index)
        {
            return source.ElementAt(index);
        }

        //[Bindable(true)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        //[Category("Data")]
        public MultiplayerGamelistData DataSource
        {
            get
            {
                return internal_source;
            }
            set
            {
                internal_source = value;
                bind();
            }
        }


        private void bind()
        {
            //this.BeginUpdate();
            //Clear the existing list
            VirtualListSize = 0;
            Items.Clear();
            Columns.Clear();
            LargeImageList = new ImageList();
            LargeImageList.ImageSize = new Size(200, 200);
            LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            SmallImageList = new ImageList();
            SmallImageList.ImageSize = new Size(16, 16);
            source = internal_source?.Sessions.Select(dr => new LinqListViewMultiplayerItem(dr)).ToList();
            if (source != null)
            {
                Columns.Add("Name", "Name", 200);
                Columns.Add("PlayerCount", "#", 50);
                Columns.Add("GameType", "GameType", 75);
                Columns.Add("GameMode", "GameMode", 75);
                Columns.Add("Map", "Map", 220);
                Columns.Add("Mod", "Mod", 220);
                Columns.Add("MotD", "MotD", 85);

                VirtualListSize = source.Count;

                /*int imageIndex = 0;
                foreach(ILinqListView2Item item in source)
                {
                    ListViewItem lvi = new ListViewItem(item.Name, item.IconKey);
                    lvi.Tag = item;
                    Items.Add(lvi);

                    if (item.Icon != null)
                    {
                        newImages.Images.Add(item.IconKey, item.Icon);
                    }

                    imageIndex++;
                }*/
            }
            else
            {
                //If no source is defined, Currency Manager is null  
                //cm = null;
            }
            this.EndUpdate();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }

    public class LinqListViewMultiplayerItem
    {
        public string IconKey { get { return SessionItem.Level.ID; } }
        public string Name { get { return SessionItem.Name; } }
        public string GameType { get { return SessionItem.Level.GameType; } }
        public string GameMode { get { return SessionItem.Level.GameMode; } }
        public string Map { get { return SessionItem.Level.Name ?? SessionItem.Level.MapFile; } }
        public string MotD { get { return SessionItem.Message; } }
        public string Mod { get { return SessionItem.Game.Mod ?? SessionItem.Level.Mod; } }
        public string PlayerCount { get { return $"{SessionItem.PlayerCount.Select(dr => dr.Value).Sum()}/{(SessionItem.PlayerTypes?.Where(dr => dr.Max.HasValue)?.Select(dr => dr.Max)?.FirstOrDefault()?.ToString() ?? " ? ")}"; } }

        public Image LargeIcon { get; set; }
        public Image SmallIcon { get; set; }
        public ListViewItem ListViewItemCache { get; set; }
        public MultiplayerGamelistData_Session SessionItem { get; }

        public LinqListViewMultiplayerItem(MultiplayerGamelistData_Session SessionItem)
        {
            this.SessionItem = SessionItem;
            try
            {
                if (SessionItem?.Level?.Image != null)
                {
                    HttpWebRequest req = WebRequest.CreateHttp(SessionItem.Level.Image);
                    using (WebResponse resp = req.GetResponse())
                    {
                        LargeIcon = Image.FromStream(resp.GetResponseStream());
                    }
                }
            }
            catch { }
        }
    }

}
