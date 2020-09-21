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
    public partial class PasswordDialog : Form
    {
        public string Password
        {
            get
            {
                return textBox1.Text;
            }
        }

        public PasswordDialog(string Title)
        {
            InitializeComponent();
            this.Text = Title;
        }
    }
}
