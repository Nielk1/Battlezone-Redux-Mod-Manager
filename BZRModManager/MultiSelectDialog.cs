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
    public partial class MultiSelectDialog : Form
    {
        public string[] Selected
        {
            get
            {
                List<string> Items = new List<string>();
                foreach (var item in checkedListBox1.CheckedItems)
                    Items.Add((string)item);
                return Items.ToArray();
            }
        }

        public MultiSelectDialog(string Title, IEnumerable<Tuple<string, bool>> ListItems)
        {
            InitializeComponent();
            this.Text = Title;
            foreach (var ListItem in ListItems)
            {
                checkedListBox1.Items.Add(ListItem.Item1, ListItem.Item2);
            }
        }
    }
}
