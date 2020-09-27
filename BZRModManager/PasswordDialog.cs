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

        private void PasswordDialog_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Password.Length > 0)
                {
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                this.Close();
            }
        }
    }
}
