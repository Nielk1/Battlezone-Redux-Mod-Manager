using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    public class MultiSelectDialogItem
    {
        public string Name { get; set; }
        public object Tag { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public partial class MultiSelectDialog : Form
    {
        public string[] Selected
        {
            get
            {
                List<string> Items = new List<string>();
                foreach (MultiSelectDialogItem item in checkedListBox1.CheckedItems)
                    Items.Add((string)item.Tag);
                return Items.ToArray();
            }
        }

        public MultiSelectDialog(string Title, IEnumerable<Tuple<string, object, bool>> ListItems)
        {
            InitializeComponent();
            this.Text = Title;
            foreach (var ListItem in ListItems)
            {
                checkedListBox1.Items.Add(new MultiSelectDialogItem()
                {
                    Name = ListItem.Item1,
                    Tag = ListItem.Item2,
                }, ListItem.Item3);
            }
        }
    }
}
