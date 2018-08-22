﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    class LinqListView : System.Windows.Forms.ListView
    {
        public LinqListView()
        {
            // This call is required by the Windows.Forms Form Designer.
            //InitializeComponent();

            DataSource = new List<ILinqListViesItem>();

            base.RetrieveVirtualItem += LinqListView_RetrieveVirtualItem;

            //base.SelectedIndexChanged += new EventHandler(
            //                   MyListView_SelectedIndexChanged);
            base.ColumnClick += new ColumnClickEventHandler(LinqListView_ColumnClick);
            base.MouseDoubleClick += LinqListView_MouseDoubleClick;
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
                            ILinqListViesItem temp = (item.Tag as ILinqListViesItem);
                            if (temp != null)
                            {
                                temp.ToggleSteam();
                                source.Where(dx => dx.WorkshopIdOutput == temp.WorkshopIdOutput).ToList().ForEach(dr => dr.ListViewItemCache = null);
                                this.Refresh();
                            }
                        }
                        if (ix == 5)
                        {
                            ILinqListViesItem temp = (item.Tag as ILinqListViesItem);
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

        private IContainer components;
        List<int> sorts = new List<int>();
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

            ApplySort();
        }
        private void ApplySort()
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

            IOrderedEnumerable<ILinqListViesItem> query = null;
            bool first = true;
            foreach (int sort in sorts)
            {
                switch (sort)
                {
                    case 1:
                        if(first) query = source.OrderBy(dr => dr.Name);
                        if(!first) query = query.ThenBy(dr => dr.Name);
                        break;
                    case -1:
                        if (first) query = source.OrderByDescending(dr => dr.Name);
                        if (!first) query = query.ThenByDescending(dr => dr.Name);
                        break;
                    case 2:
                        if (first) query = source.OrderBy(dr => dr.ModType);
                        if (!first) query = query.ThenBy(dr => dr.ModType);
                        break;
                    case -2:
                        if (first) query = source.OrderByDescending(dr => dr.ModType);
                        if (!first) query = query.ThenByDescending(dr => dr.ModType);
                        break;
                    case 3:
                        if (first) query = source.OrderBy(dr => dr.ModSource);
                        if (!first) query = query.ThenBy(dr => dr.ModSource);
                        break;
                    case -3:
                        if (first) query = source.OrderByDescending(dr => dr.ModSource);
                        if (!first) query = query.ThenByDescending(dr => dr.ModSource);
                        break;
                    case 4:
                        if (first) query = source.OrderBy(dr => dr.WorkshopIdOutput);
                        if (!first) query = query.ThenBy(dr => dr.WorkshopIdOutput);
                        break;
                    case -4:
                        if (first) query = source.OrderByDescending(dr => dr.WorkshopIdOutput);
                        if (!first) query = query.ThenByDescending(dr => dr.WorkshopIdOutput);
                        break;
                    case 5:
                        if (first) query = source.OrderBy(dr => dr.InstalledSteam);
                        if (!first) query = query.ThenBy(dr => dr.InstalledSteam);
                        break;
                    case -5:
                        if (first) query = source.OrderByDescending(dr => dr.InstalledSteam);
                        if (!first) query = query.ThenByDescending(dr => dr.InstalledSteam);
                        break;
                    case 6:
                        if (first) query = source.OrderBy(dr => dr.InstalledGog);
                        if (!first) query = query.ThenBy(dr => dr.InstalledGog);
                        break;
                    case -6:
                        if (first) query = source.OrderByDescending(dr => dr.InstalledGog);
                        if (!first) query = query.ThenByDescending(dr => dr.InstalledGog);
                        break;
                }
                first = false;
            }
            source = query?.ToList() ?? source;
            this.Refresh();
        }

        private void LinqListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            //ILinqListViesItem item = source[e.ItemIndex];
            ILinqListViesItem item = source.ElementAt(e.ItemIndex);

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
                case InstallStatus.Uninstalled:
                    stat1 = lvi.SubItems.Add("N");
                    stat1.BackColor = Color.Red;
                    break;
                case InstallStatus.ForceDisabled:
                    stat1 = lvi.SubItems.Add("N");
                    stat1.BackColor = Color.Pink;
                    stat1.ForeColor = Color.Gray;
                    break;
                case InstallStatus.ForceEnabled:
                    stat1 = lvi.SubItems.Add("Y");
                    stat1.BackColor = Color.LightGreen;
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
            lvi.SubItems.Add(string.Join(",", item.ModTags));
            //lvi.SubItems.Add(item.Version);
            //lvi.SubItems.Add(item.Vendor);
            //lvi.SubItems.Add(item.NokiaCategory);
            //lvi.SubItems.Add(item.ScreenSize);
            //lvi.SubItems.Add(item.FileName);
            e.Item = lvi;

            item.ListViewItemCache = lvi;
        }

        private List<ILinqListViesItem> source;

        //[Bindable(true)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        //[Category("Data")]
        public List<ILinqListViesItem> DataSource
        {
            get
            {
                return source;
            }
            set
            {
                if (source != null)
                {
                    source.Clear();
                    if (value != null)
                    {
                        source.AddRange(value);
                    }
                }
                else
                {
                    source = value;
                }
                bind();
                ApplySort();
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
                foreach(ILinqListViesItem item in source)
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

    public interface ILinqListViesItem
    {
        string IconKey { get; }
        string Name { get; }

        string ModType { get; }
        string[] ModTags { get; }
        string WorkshopIdOutput { get; }
        string ModSource { get; }

        InstallStatus InstalledSteam { get; }
        InstallStatus InstalledGog { get; }

        //string FileName { get; }
        //string WebName { get; }
        Image LargeIcon { get; }
        Image SmallIcon { get; }
        //string Version { get; }
        //string Vendor { get; }
        //string NokiaCategory { get; }
        //string ScreenSize { get; }
        ListViewItem ListViewItemCache { get; set; }

        void ToggleGog();
        void ToggleSteam();
    }

}
