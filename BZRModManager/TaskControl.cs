using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace BZRModManager
{
    public partial class TaskControl : UserControl
    {
        private int _value;
        private int _maximum;
        private string _text = string.Empty;

        /// <summary>
        /// Queue a UI-only operation without blocking the calling worker thread.
        /// This must only be used after this TaskControl has been created on and
        /// attached to the application's UI thread.
        /// </summary>
        private void UiPost(Action action)
        {
            if (action == null || IsDisposed || Disposing)
                return;

            try
            {
                void RunIfAlive()
                {
                    if (!IsDisposed && !Disposing)
                        action();
                }

                if (InvokeRequired)
                    BeginInvoke((Action)RunIfAlive);
                else
                    RunIfAlive();
            }
            catch (ObjectDisposedException)
            {
                // Application/control teardown raced the queued update.
            }
            catch (InvalidOperationException)
            {
                // Handle/control teardown raced the queued update.
            }
        }

        /// <summary>
        /// Execute a UI-only operation synchronously and return its result.
        /// Used only when the caller needs the result before it can continue,
        /// such as constructing a child TaskControl.
        /// </summary>
        private T UiInvoke<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            if (IsDisposed || Disposing)
                throw new ObjectDisposedException(nameof(TaskControl));

            if (InvokeRequired)
                return (T)Invoke(func);

            return func();
        }

        public override string Text
        {
            get => Volatile.Read(ref _text);
            set
            {
                Interlocked.Exchange(ref _text, value ?? string.Empty);
                UiPost(ApplyTextState);
            }
        }

        private void ApplyTextState()
        {
            lblText.Text = Volatile.Read(ref _text);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            // Do not read the ProgressBar from a worker thread.  The backing
            // value is the logical state; the ProgressBar is only its UI view.
            get => Volatile.Read(ref _value);
            set
            {
                Interlocked.Exchange(ref _value, value);
                UiPost(ApplyProgressState);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
        {
            // Same rule as Value: this property is safe to use from workers and
            // the actual WinForms control is updated only on the UI thread.
            get => Volatile.Read(ref _maximum);
            set
            {
                Interlocked.Exchange(ref _maximum, value);
                UiPost(ApplyProgressState);
            }
        }

        private void ApplyProgressState()
        {
            int maximum = Volatile.Read(ref _maximum);
            int value = Volatile.Read(ref _value);

            if (maximum > 0)
            {
                pbProg.Style = ProgressBarStyle.Blocks;

                // If Maximum is being reduced, first move the displayed value
                // into the new range so ProgressBar cannot reject the change.
                if (pbProg.Value > maximum)
                    pbProg.Value = maximum;

                pbProg.Maximum = maximum;
                pbProg.Value = Math.Max(pbProg.Minimum, Math.Min(value, maximum));
            }
            else
            {
                pbProg.Maximum = 100;
                pbProg.Value = 100;
                pbProg.Style = ProgressBarStyle.Marquee;
            }
        }

        private int baseHeight;

        /// <summary>
        /// IMPORTANT: TaskControl instances must be constructed on the WinForms
        /// UI thread. MainForm.AddTask() and TaskControl.AddTask() are responsible
        /// for enforcing that rule.
        /// </summary>
        public TaskControl(string text, int maximum)
        {
            InitializeComponent();

            // The constructor itself is running on the UI thread, so initialize
            // the actual child controls directly instead of posting work.
            _text = text ?? string.Empty;
            _maximum = maximum;
            _value = 0;

            ApplyTextState();
            ApplyProgressState();

            baseHeight = Height;
        }

        /// <summary>
        /// Create and attach a child task.  If called from a worker, the *entire*
        /// construction is marshalled to this TaskControl's owning UI thread.
        /// This is critical: constructing a WinForms Control on a worker can
        /// install a WindowsFormsSynchronizationContext on that worker.
        /// </summary>
        public TaskControl AddTask(string name, int maxValue)
        {
            return UiInvoke(() =>
            {
                TaskControl ctrl = new TaskControl(name, maxValue)
                {
                    Margin = new Padding(0)
                };

                pnlTasks.Controls.Add(ctrl);
                FixHeightCore();
                Refresh();

                return ctrl;
            });
        }

        /// <summary>
        /// Remove a child task on the UI thread.  Removal is fire-and-forget so
        /// background work does not block merely to update the task display.
        /// </summary>
        public void EndTask(TaskControl ctrl)
        {
            if (ctrl == null)
                return;

            UiPost(() =>
            {
                pnlTasks.Controls.Remove(ctrl);
                FixHeightCore();
                Refresh();
            });
        }

        /// <summary>
        /// Public thread-safe wrapper.  The actual recursive WinForms traversal
        /// is always performed on the owning UI thread.
        /// </summary>
        public int FixHeight()
        {
            return UiInvoke(FixHeightCore);
        }

        private int FixHeightCore()
        {
            int totalHeight = baseHeight;

            foreach (Control control in pnlTasks.Controls)
            {
                if (control is TaskControl taskControl)
                    totalHeight += taskControl.FixHeightCore();
            }

            Height = totalHeight;
            return totalHeight;
        }
    }
}
