using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace BZRModManager
{
    class LinqListView2 : System.Windows.Forms.ListView
    {
        public LinqListView2()
        {
            // This call is required by the Windows.Forms Form Designer.
            //InitializeComponent();

            DataSource = new List<ILinqListView2Item>();

            base.RetrieveVirtualItem += LinqListView2_RetrieveVirtualItem;

            //base.SelectedIndexChanged += new EventHandler(
            //                   MyListView_SelectedIndexChanged);
            base.ColumnClick += new ColumnClickEventHandler(LinqListView2_ColumnClick);
            base.MouseDoubleClick += LinqListView2_MouseDoubleClick;
            _resizeTimer.Tick += _resizeTimer_Tick;
            base.Resize += LinqListView2_Resize;
            base.ColumnWidthChanging += LinqListView2_ColumnWidthChanging;
            base.ColumnWidthChanged += LinqListView2_ColumnWidthChanged;
        }

        private void LinqListView2_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            ListChangedRecently = false;
            this.Invalidate();
        }

        private void LinqListView2_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ListChangedRecently = true;
        }

        bool ListChangedRecently = false;
        DispatcherTimer _resizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500), IsEnabled = false };
        private void LinqListView2_Resize(object sender, EventArgs e)
        {
            _resizeTimer.Stop();
            ListChangedRecently = true;
            _resizeTimer.IsEnabled = true;
            _resizeTimer.Start();
        }
        void _resizeTimer_Tick(object sender, EventArgs e)
        {
            _resizeTimer.IsEnabled = false;
            ListChangedRecently = false;
            this.Invalidate();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public uint idFrom;
            public uint code;
        }

        private const uint NM_CUSTOMDRAW = unchecked((uint)-12);

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                //case 0x0214: // WM_SIZING
                //case 0x0005: // WM_SIZE
                //    {
                //        ListChangedRecently = true;
                //    }
                //    break;
                //case 0x0232: // WM_EXITSIZEMOVE
                //    {
                //        //ListChangedRecently = false;
                //        //this.Invalidate();
                //    }
                //    break;
                case 0x115: // Trap WM_VSCROLL
                    {
                        ScrollEventType ScrollType = (ScrollEventType)(m.WParam.ToInt32() & 0xffff);
                        if (ScrollType == ScrollEventType.EndScroll)
                        {
                            ListChangedRecently = false;
                            this.Invalidate();
                        }
                        else
                        {
                            ListChangedRecently = true;
                        }
                    }
                    break;
                case 0x204E:
                    {
                        if (ListChangedRecently)
                        {
                            NMHDR hdr = (NMHDR)m.GetLParam(typeof(NMHDR));
                            if (hdr.code == NM_CUSTOMDRAW)
                            {
                                m.Result = (IntPtr)0;
                                return;
                            }
                        }
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        private void LinqListView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                ListViewItem item = this.GetItemAt(5, e.Y);
                if (item == null) return;
            }
        }

        List<int> sorts = new List<int>();
        public List<string> TypeFilter { get { return _TypeFilter; } set { _TypeFilter = value; ApplySortAndFilter(); } }
        private List<string> _TypeFilter;
        private void LinqListView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == 6) return;
            int sortCol = (e.Column + 1);
            if (sorts.Count == 0)
            {
                sorts.Add(sortCol);
            }
            else
            {
                if (sorts[0] == sortCol)
                {
                    sorts[0] = -sorts[0];
                }
                else
                {
                    sorts.Remove(sortCol);
                    sorts.Remove(-sortCol);
                    sorts.Insert(0, sortCol);
                }
            }

            /*for (int i = 0; i < 2; i++)
            {
                if (sorts.Contains(i + 1))
                {
                    this.SetSortIcon(i, SortOrder.Ascending);
                }
                else if (sorts.Contains(-(i + 1)))
                {
                    this.SetSortIcon(i, SortOrder.Descending);
                }
                else
                {
                    this.SetSortIcon(i, SortOrder.None);
                }
            }*/

            ApplySortAndFilter();
        }
        private void ApplySortAndFilter()
        {
            Console.WriteLine($"Sorting by {string.Join(",", sorts)}");

            if (sorts.Count > 0)
            {
                int sign = Math.Sign(sorts[0]);
                int column = Math.Abs(sorts[0]) - 1;
                if (sign == 1)
                {
                    this.SetSortIcon(column, SortOrder.Ascending);
                }
                else if (sign == -1)
                {
                    this.SetSortIcon(column, SortOrder.Descending);
                }
                else
                {
                    this.SetSortIcon(column, SortOrder.None);
                }
            }

            IOrderedEnumerable<ILinqListView2Item> query = null;
            bool first = true;
            foreach (int sort in sorts)
            {
                switch (sort)
                {
                    case 1:
                        if(first) query = internal_source.OrderBy(dr => dr.Name);
                        if(!first) query = query.ThenBy(dr => dr.Name);
                        break;
                    case -1:
                        if (first) query = internal_source.OrderByDescending(dr => dr.Name);
                        if (!first) query = query.ThenByDescending(dr => dr.Name);
                        break;
                    case 2:
                        if (first) query = internal_source.OrderBy(dr => dr.Author);
                        if (!first) query = query.ThenBy(dr => dr.Author);
                        break;
                    case -2:
                        if (first) query = internal_source.OrderByDescending(dr => dr.Author);
                        if (!first) query = query.ThenByDescending(dr => dr.Author);
                        break;
                    case 3:
                        if (first) query = internal_source.OrderBy(dr => dr.ModSource);
                        if (!first) query = query.ThenBy(dr => dr.ModSource);
                        break;
                    case -3:
                        if (first) query = internal_source.OrderByDescending(dr => dr.ModSource);
                        if (!first) query = query.ThenByDescending(dr => dr.ModSource);
                        break;
                }
                first = false;
            }
            source = query?.ToList() ?? internal_source;
            if (TypeFilter != null)
            {
                source = source.Where(dr => TypeFilter.Any(dx => dr.Tags?.Contains(dx) ?? false)).ToList();
            }
            VirtualListSize = source.Count;
            this.Refresh();
        }

        private void LinqListView2_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //ILinqListView2Item item = source[e.ItemIndex];
            ILinqListView2Item item = source.ElementAt(e.ItemIndex);

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
            lvi.SubItems.Add(item.Author);
            lvi.SubItems.Add(item.ModSource);
            e.Item = lvi;

            item.ListViewItemCache = lvi;
        }

        private List<ILinqListView2Item> source;
        private List<ILinqListView2Item> internal_source;

        public ILinqListView2Item GetItemAtVirtualIndex(int index)
        {
            return source.ElementAt(index);
        }

        //[Bindable(true)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        //[Category("Data")]
        public List<ILinqListView2Item> DataSource
        {
            get
            {
                return internal_source;
            }
            set
            {
                if (internal_source != null)
                {
                    internal_source.Clear();
                    if (value != null)
                    {
                        internal_source.AddRange(value);
                    }
                }
                else
                {
                    internal_source = value;
                }
                bind();
                ApplySortAndFilter();
            }
        }


        /*[Browsable(false)]
        public new SortOrder Sorting
        {
            get
            {
                return base.Sorting;
            }
            set
            {
                base.Sorting = value;
            }
        }*/

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
            if (source != null)
            {
                Columns.Add("Name", "Name", 350);
                Columns.Add("Author", "Author", 85);
                Columns.Add("ModSource", "Source", 85);

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

    public interface ILinqListView2Item
    {
        string IconKey { get; }
        string Name { get; }
        string Author { get; }

        string ModSource { get; }
        string[] Tags { get; }

        string URL { get; }
        Image LargeIcon { get; }
        Image SmallIcon { get; }
        ListViewItem ListViewItemCache { get; set; }
    }

}
