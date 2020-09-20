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
    class LinqListViewPlayers : System.Windows.Forms.ListView
    {
        public LinqListViewPlayers()
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
            LinqListViewPlayersItem item = source.ElementAt(e.ItemIndex);

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
            e.Item = lvi;

            item.ListViewItemCache = lvi;
        }

        private List<LinqListViewPlayersItem> source;
        private List<MultiplayerGamelistData_Session_Player> internal_source;

        public LinqListViewPlayersItem GetItemAtVirtualIndex(int index)
        {
            return source.ElementAt(index);
        }

        //[Bindable(true)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        //[Category("Data")]
        public List<MultiplayerGamelistData_Session_Player> DataSource
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
            LargeImageList.ImageSize = new Size(64, 64);
            LargeImageList.ColorDepth = ColorDepth.Depth32Bit;
            SmallImageList = new ImageList();
            SmallImageList.ImageSize = new Size(16, 16);
            source = internal_source?.Select(dr => new LinqListViewPlayersItem(dr)).ToList();
            //if (source != null)
            {
                Columns.Add("Name", "Name", 200);

                VirtualListSize = source?.Count ?? 0;

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
            //else
            //{
            //    //If no source is defined, Currency Manager is null  
            //    //cm = null;
            //}
            this.EndUpdate();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }

    public class LinqListViewPlayersItem
    {
        public string IconKey { get { return (string)(PlayerItem.IDs["BZRNet"].ID); } }
        public string Name { get { return PlayerItem.Name; } }
        public string URL { get { return PlayerItem?.IDs?.Select(dr => dr.Value?.ProfileUrl)?.Where(dr => dr != null)?.FirstOrDefault(); } }

        public Image LargeIcon { get; set; }
        public Image SmallIcon { get; set; }
        public ListViewItem ListViewItemCache { get; set; }
        public MultiplayerGamelistData_Session_Player PlayerItem { get; }

        public LinqListViewPlayersItem(MultiplayerGamelistData_Session_Player PlayerItem)
        {
            this.PlayerItem = PlayerItem;
            try
            {
                string Avatar = PlayerItem?.IDs?.Select(dr => dr.Value?.AvatarUrl)?.Where(dr => dr != null)?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(Avatar))
                {
                    HttpWebRequest req = WebRequest.CreateHttp(Avatar);
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
