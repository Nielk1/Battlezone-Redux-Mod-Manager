using BZRModManager.ModItem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace BZRModManager
{
    class LinqListViewMods : System.Windows.Forms.ListView
    {
        public LinqListViewMods()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            DataSource = new List<ILinqListViewItemMods>();

            base.RetrieveVirtualItem += LinqListView_RetrieveVirtualItem;

            //base.SelectedIndexChanged += new EventHandler(
            //                   MyListView_SelectedIndexChanged);
            base.ColumnClick += new ColumnClickEventHandler(LinqListView_ColumnClick);
            base.MouseDoubleClick += LinqListView_MouseDoubleClick;
            base.MouseClick += LinqListViewMods_MouseClick;
            _resizeTimer.Tick += _resizeTimer_Tick;
            base.Resize += LinqListView_Resize;
            base.ColumnWidthChanging += LinqListView_ColumnWidthChanging;
            base.ColumnWidthChanged += LinqListView_ColumnWidthChanged;
        }

        private void LinqListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            ListChangedRecently = false;
            this.Invalidate();
        }

        private void LinqListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            ListChangedRecently = true;
        }

        bool ListChangedRecently = false;
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;
        private ToolStripMenuItem tsmInstallGog;
        private ToolStripMenuItem tsmInstallSteam;
        private ToolStripMenuItem tsmUninstallGog;
        private ToolStripMenuItem tsmUninstallSteam;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem tsmOpenFolder;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem tsmDelete;
        DispatcherTimer _resizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 500), IsEnabled = false };
        private void LinqListView_Resize(object sender, EventArgs e)
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

        private void LinqListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                ListViewItem item = this.GetItemAt(5, e.Y);
                if (item == null) return;
                for (int ix = item.SubItems.Count - 1; ix >= 0; --ix)
                    if (item.SubItems[ix].Bounds.Contains(e.Location))
                    {
                        if (ix == 4)
                        {
                            ILinqListViewItemMods temp = (item.Tag as ILinqListViewItemMods);
                            if (temp != null)
                            {
                                temp.ToggleSteam();
                                source.Where(dx => dx.WorkshopIdOutput == temp.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                                this.Refresh();
                            }
                        }
                        if (ix == 5)
                        {
                            ILinqListViewItemMods temp = (item.Tag as ILinqListViewItemMods);
                            if (temp != null)
                            {
                                temp.ToggleGog();
                                source.Where(dx => dx.WorkshopIdOutput == temp.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                                this.Refresh();
                            }
                        }
                        break;
                    }
            }
        }

        private void LinqListViewMods_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var SelectedItems = this.SelectedIndices;
                if (SelectedItems.Count > 0)
                {
                    contextMenuStrip1.Show(this, e.Location);
                }
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            contextMenuStrip1.Close();
            if (e.ClickedItem == tsmInstallGog)
            {
                foreach (int idx in this.SelectedIndices)
                {
                    var item = source.ElementAt(idx);
                    if (item.InstalledGog != InstallStatus.Linked)
                    {
                        item.ToggleGog(); // this won't do anything unless we can, so it's safe to just crudely do this
                        source.Where(dx => dx.WorkshopIdOutput == item.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                    }
                }
                this.Refresh();
            }
            if (e.ClickedItem == tsmInstallSteam)
            {
                foreach (int idx in this.SelectedIndices)
                {
                    var item = source.ElementAt(idx);
                    if (item.InstalledSteam != InstallStatus.Linked)
                    {
                        item.ToggleSteam(); // this won't do anything unless we can, so it's safe to just crudely do this
                        source.Where(dx => dx.WorkshopIdOutput == item.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                    }
                }
                this.Refresh();
            }
            if (e.ClickedItem == tsmUninstallGog)
            {
                foreach (int idx in this.SelectedIndices)
                {
                    var item = source.ElementAt(idx);
                    if (item.InstalledGog == InstallStatus.Linked)
                    {
                        item.ToggleGog(); // this won't do anything unless we can, so it's safe to just crudely do this
                        source.Where(dx => dx.WorkshopIdOutput == item.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                    }
                }
                this.Refresh();
            }
            if (e.ClickedItem == tsmUninstallSteam)
            {
                foreach (int idx in this.SelectedIndices)
                {
                    var item = source.ElementAt(idx);
                    if (item.InstalledSteam == InstallStatus.Linked)
                    {
                        item.ToggleSteam(); // this won't do anything unless we can, so it's safe to just crudely do this
                        source.Where(dx => dx.WorkshopIdOutput == item.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                    }
                }
                this.Refresh();
            }
            if (e.ClickedItem == tsmOpenFolder)
            {
                SelectedIndexCollection SelectedItems = this.SelectedIndices;
                if (SelectedItems.Count > 0)
                {
                    if (SelectedItems.Count < 5 || MessageBox.Show($"Open {SelectedItems.Count} Folders?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        foreach (int idx in SelectedItems)
                        {
                            var item = source.ElementAt(idx);
                            if (!string.IsNullOrWhiteSpace(item.FilePath) && Directory.Exists(item.FilePath))
                                Process.Start("explorer.exe", item.FilePath);
                        }
                    }
                }
            }
            if (e.ClickedItem == tsmDelete)
            {
                SelectedIndexCollection SelectedItems = this.SelectedIndices;
                if (SelectedItems.Count > 0)
                {
                    if (MessageBox.Show($"Delete these {SelectedItems.Count} mod files?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        bool DidDelete = false;
                        foreach (int idx in SelectedItems)
                        {
                            var item = source.ElementAt(idx);
                            if (item.InstalledGog == InstallStatus.Linked)
                                item.ToggleGog();
                            if (item.InstalledSteam == InstallStatus.Linked)
                                item.ToggleSteam();
                            source.Where(dx => dx.WorkshopIdOutput == item.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                            DidDelete |= item.Delete();
                        }

                        if (DidDelete)
                            MessageBox.Show($"Mods deleted, please refresh the mod list.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                this.Refresh();
            }
        }

        List<int> sorts = new List<int>();
        public List<string> TypeFilter { get { return _TypeFilter; } set { _TypeFilter = value; ApplySortAndFilter(); } }
        private List<string> _TypeFilter;
        private void LinqListView_ColumnClick(object sender, ColumnClickEventArgs e)
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

            IOrderedEnumerable<ILinqListViewItemMods> query = null;
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
                        if (first) query = internal_source.OrderBy(dr => dr.ModType);
                        if (!first) query = query.ThenBy(dr => dr.ModType);
                        break;
                    case -2:
                        if (first) query = internal_source.OrderByDescending(dr => dr.ModType);
                        if (!first) query = query.ThenByDescending(dr => dr.ModType);
                        break;
                    case 3:
                        if (first) query = internal_source.OrderBy(dr => dr.ModSource);
                        if (!first) query = query.ThenBy(dr => dr.ModSource);
                        break;
                    case -3:
                        if (first) query = internal_source.OrderByDescending(dr => dr.ModSource);
                        if (!first) query = query.ThenByDescending(dr => dr.ModSource);
                        break;
                    case 4:
                        if (first) query = internal_source.OrderBy(dr => dr.WorkshopIdOutput);
                        if (!first) query = query.ThenBy(dr => dr.WorkshopIdOutput);
                        break;
                    case -4:
                        if (first) query = internal_source.OrderByDescending(dr => dr.WorkshopIdOutput);
                        if (!first) query = query.ThenByDescending(dr => dr.WorkshopIdOutput);
                        break;
                    case 5:
                        if (first) query = internal_source.OrderBy(dr => dr.InstalledSteam);
                        if (!first) query = query.ThenBy(dr => dr.InstalledSteam);
                        break;
                    case -5:
                        if (first) query = internal_source.OrderByDescending(dr => dr.InstalledSteam);
                        if (!first) query = query.ThenByDescending(dr => dr.InstalledSteam);
                        break;
                    case 6:
                        if (first) query = internal_source.OrderBy(dr => dr.InstalledGog);
                        if (!first) query = query.ThenBy(dr => dr.InstalledGog);
                        break;
                    case -6:
                        if (first) query = internal_source.OrderByDescending(dr => dr.InstalledGog);
                        if (!first) query = query.ThenByDescending(dr => dr.InstalledGog);
                        break;
                }
                first = false;
            }
            source = query?.ToList() ?? internal_source;
            if (TypeFilter != null)
            {
                source = source.Where(dr => TypeFilter.Any(dx => dr.ModType.Contains(dx))).ToList();
            }
            VirtualListSize = source.Count;
            this.Refresh();
        }

        private void LinqListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //ILinqListViewItem item = source[e.ItemIndex];
            ILinqListViewItemMods item = source.ElementAt(e.ItemIndex);

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
            lvi.SubItems.Add(item.ModType);
            lvi.SubItems.Add(item.ModSource);
            lvi.SubItems.Add(item.WorkshopIdOutput);
            ListViewItem.ListViewSubItem stat1 = null;
            ListViewItem.ListViewSubItem stat2 = null;
            switch (item.InstalledSteam)
            {
                case InstallStatus.Unknown:
                    stat1 = lvi.SubItems.Add(string.Empty);
                    break;
                case InstallStatus.Missing:
                    stat1 = lvi.SubItems.Add("M");
                    stat1.BackColor = Color.Orange;
                    break;
                case InstallStatus.Uninstalled:
                    stat1 = lvi.SubItems.Add("N");
                    stat1.BackColor = Color.Red;
                    break;
                case InstallStatus.ForceDisabled:
                    stat1 = lvi.SubItems.Add("N");
                    //stat1.BackColor = Color.Pink;
                    stat1.ForeColor = Color.Gray;
                    break;
                case InstallStatus.ForceEnabled:
                    stat1 = lvi.SubItems.Add("Y");
                    //stat1.BackColor = Color.LightGreen;
                    stat1.ForeColor = Color.Gray;
                    break;
                case InstallStatus.Linked:
                    stat1 = lvi.SubItems.Add("Y");
                    stat1.BackColor = Color.Green;
                    break;
                case InstallStatus.Collision:
                    stat1 = lvi.SubItems.Add("C");
                    stat1.BackColor = Color.Purple;
                    break;
            }
            switch (item.InstalledGog)
            {
                case InstallStatus.Unknown:
                    stat2 = lvi.SubItems.Add(string.Empty);
                    break;
                case InstallStatus.Missing:
                    stat2 = lvi.SubItems.Add("X");
                    break;
                case InstallStatus.Uninstalled:
                    stat2 = lvi.SubItems.Add("N");
                    stat2.BackColor = Color.Red;
                    break;
                case InstallStatus.ForceDisabled:
                    stat2 = lvi.SubItems.Add("N");
                    stat2.BackColor = Color.Pink;
                    stat2.ForeColor = Color.Gray;
                    break;
                case InstallStatus.ForceEnabled:
                    stat2 = lvi.SubItems.Add("Y");
                    stat2.BackColor = Color.LightGreen;
                    stat2.ForeColor = Color.Gray;
                    break;
                case InstallStatus.Linked:
                    stat2 = lvi.SubItems.Add("Y");
                    stat2.BackColor = Color.Green;
                    break;
                case InstallStatus.Collision:
                    stat2 = lvi.SubItems.Add("C");
                    stat2.BackColor = Color.Purple;
                    break;
            }
            lvi.SubItems.Add(item.ModTags != null ? string.Join(",", item.ModTags) : string.Empty);
            //lvi.SubItems.Add(item.Version);
            //lvi.SubItems.Add(item.Vendor);
            //lvi.SubItems.Add(item.NokiaCategory);
            //lvi.SubItems.Add(item.ScreenSize);
            //lvi.SubItems.Add(item.FileName);
            e.Item = lvi;

            item.ListViewItemCache = lvi;
        }

        private List<ILinqListViewItemMods> source;
        private List<ILinqListViewItemMods> internal_source;

        //[Bindable(true)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        //[Category("Data")]
        public List<ILinqListViewItemMods> DataSource
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
            LargeImageList.ImageSize = new Size(80, 64);
            SmallImageList = new ImageList();
            SmallImageList.ImageSize = new Size(16, 16);
            if (source != null)
            {
                Columns.Add("Name", "Name", 350);
                Columns.Add("ModType", "Type", 85);
                Columns.Add("ModSource", "Source", 85);
                Columns.Add("WorkshopIdOutput", "WorkshopID", 85);
                Columns.Add("InstalledSteam", "Steam", 45, HorizontalAlignment.Center, 0);
                Columns.Add("InstalledGog", "GOG", 45, HorizontalAlignment.Center, 0);
                Columns.Add("ModTags", "Tags", 200);
                //Columns.Add("WebName", "Web Name", 200);
                //Columns.Add("Version", "Version", 50);
                //Columns.Add("Vendor", "Vendor", 150);
                //Columns.Add("NokiaCategory", "Nokia Category", 100);
                //Columns.Add("ScreenSize", "Screen Size", 100);
                //Columns.Add("FileName", "File Name", 200);

                VirtualListSize = source.Count;

                /*int imageIndex = 0;
                foreach(ILinqListViewItem item in source)
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmInstallGog = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmInstallSteam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUninstallGog = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUninstallSteam = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmInstallGog,
            this.tsmInstallSteam,
            this.tsmUninstallGog,
            this.tsmUninstallSteam,
            this.toolStripSeparator1,
            this.tsmOpenFolder,
            this.toolStripSeparator2,
            this.tsmDelete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(157, 142);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // tsmInstallGog
            // 
            this.tsmInstallGog.Name = "tsmInstallGog";
            this.tsmInstallGog.Size = new System.Drawing.Size(156, 22);
            this.tsmInstallGog.Text = "Install GOG";
            // 
            // tsmInstallSteam
            // 
            this.tsmInstallSteam.Name = "tsmInstallSteam";
            this.tsmInstallSteam.Size = new System.Drawing.Size(156, 22);
            this.tsmInstallSteam.Text = "Install Steam";
            // 
            // tsmUninstallGog
            // 
            this.tsmUninstallGog.Name = "tsmUninstallGog";
            this.tsmUninstallGog.Size = new System.Drawing.Size(156, 22);
            this.tsmUninstallGog.Text = "Uninstall GOG";
            // 
            // tsmUninstallSteam
            // 
            this.tsmUninstallSteam.Name = "tsmUninstallSteam";
            this.tsmUninstallSteam.Size = new System.Drawing.Size(156, 22);
            this.tsmUninstallSteam.Text = "Uninstall Steam";
            // 
            // tsmDelete
            // 
            this.tsmDelete.Name = "tsmDelete";
            this.tsmDelete.Size = new System.Drawing.Size(156, 22);
            this.tsmDelete.Text = "Delete";
            // 
            // tsmOpenFolder
            // 
            this.tsmOpenFolder.Name = "tsmOpenFolder";
            this.tsmOpenFolder.Size = new System.Drawing.Size(156, 22);
            this.tsmOpenFolder.Text = "Open Folder";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(153, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(153, 6);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
