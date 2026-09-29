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
        /// <summary>
        /// Marshals <paramref name="action"/> to this control's UI thread *asynchronously* (BeginInvoke)
        /// so a calling (worker) thread is never blocked waiting on the UI thread. This removes the
        /// synchronous Control.Invoke that was part of the SteamCmd output stall/deadlock.
        /// Runs inline when already on the UI thread and silently no-ops if the control is being torn down.
        /// </summary>
        private void UiInvoke(Action action)
        {
            if (action == null) return;
            try
            {
                if (this.InvokeRequired) this.BeginInvoke(action);
                else action();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public override string Text
        {
            get
            {
                return lblText.Text;
            }
            set
            {
                UiInvoke(() =>
                {
                    lblText.Text = value;
                });
            }
        }

        private int _Value;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            get
            {
                return pbProg.Value;
            }
            set
            {
                UiInvoke(() =>
                {
                    pbProg.Value = value;
                });

                _Value = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
        {
            get
            {
                return pbProg.Maximum;
            }
            set
            {
                UiInvoke(() =>
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
        }

        private int baseHeight;

        public TaskControl(string Text, int Maximum)
        {
            InitializeComponent();
            this.Text = Text;
            this.Maximum = Maximum;
            baseHeight = this.Height;
        }

        public TaskControl AddTask(string Name, int MaxValue)
        {
            TaskControl ctrl = new TaskControl(Name, MaxValue);
            UiInvoke(() =>
            {
                ctrl.Margin = new Padding(0);
                pnlTasks.Controls.Add(ctrl);
                FixHeight();
                //pnlTasks.Refresh();
                this.Refresh();
            });
            return ctrl;
        }
        public void EndTask(TaskControl ctrl)
        {
            if (ctrl != null)
                UiInvoke(() =>
                {
                    pnlTasks.Controls.Remove(ctrl);
                    FixHeight();
                    //pnlTasks.Refresh();
                    this.Refresh();
                });
        }

        public int FixHeight()
        {
            int totalHeight = baseHeight;
            foreach(var control in pnlTasks.Controls)
            {
                totalHeight += (control as TaskControl).FixHeight();
            }
            this.Height = totalHeight;
            return totalHeight;
        }
    }
}
