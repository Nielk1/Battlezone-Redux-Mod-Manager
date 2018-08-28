using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BZRModManager
{
    public partial class TaskControl : UserControl
    {
        public override string Text
        {
            get
            {
                return lblText.Text;
            }
            set
            {
                if (this.Created)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblText.Text = value;
                    });
                }
                else
                {
                    lblText.Text = value;
                }
            }
        }

        private int _Value;
        public int Value
        {
            get
            {
                return pbProg.Value;
            }
            set
            {
                if (this.Created)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        pbProg.Value = value;
                    });
                }
                else
                {
                    pbProg.Value = value;
                }

                _Value = value;
            }
        }

        public int Maximum
        {
            get
            {
                return pbProg.Maximum;
            }
            set
            {
                if (this.Created)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (value > 0)
                        {
                            pbProg.Maximum = value;
                            pbProg.Value = _Value;
                            pbProg.Style = ProgressBarStyle.Blocks;
                        }
                        else
                        {
                            pbProg.Maximum = 100;
                            pbProg.Value = 100;
                            pbProg.Style = ProgressBarStyle.Marquee;
                        }
                    });
                }
                else
                {
                    if (value > 0)
                    {
                        pbProg.Maximum = value;
                        pbProg.Value = _Value;
                        pbProg.Style = ProgressBarStyle.Blocks;
                    }
                    else
                    {
                        pbProg.Maximum = 100;
                        pbProg.Value = 100;
                        pbProg.Style = ProgressBarStyle.Marquee;
                    }
                }
            }
        }

        public TaskControl(string Text, int Maximum)
        {
            InitializeComponent();
            this.Text = Text;
            this.Maximum = Maximum;
        }
    }
}
