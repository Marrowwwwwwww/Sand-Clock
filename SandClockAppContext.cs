using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandClock
{

    internal class SandClockAppContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private Settings settings;
        private ToolStripLabel toolStripLabel;
        private ToolStripMenuItem toolStripResetButton;
        private List<ToolStripMenuItem> toolStripDurationButtonsList = new();

        public Action OnClicked;
        public Action<float> OnSetDuration;
        public Action OnSetNoDuration;
        public Action OnExit;
        public Action OnReset;


        public NotifyIcon GetTrayIcon()
        {
            return trayIcon;
        }

        public void Refresh()
        {
            if (!settings.Active)
            {
                this.toolStripResetButton.Enabled = false;
                this.toolStripLabel.Text = "";
                trayIcon.Text = $"Sand Clock is off";
                return;
            }

            this.toolStripResetButton.Enabled = true;

            string time = TimeToReadableString(settings.flTimeInHours);
            string timeStrFull = $"Duration: {time}";
            toolStripLabel.Text = timeStrFull;
            trayIcon.Text = timeStrFull;

            // make the toolstrip resize according to the label width
            this.trayIcon.ContextMenuStrip.PerformLayout();
            this.trayIcon.ContextMenuStrip.Invalidate();
        }

        public SandClockAppContext(Settings settings)
        {
            this.settings = settings;

            trayIcon = new NotifyIcon()
            {
                Icon = IconResources.GetDefaultIcon(),
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items = {
                    }
                },
                Visible = true
            };

            ToolStrip toolStrip = trayIcon.ContextMenuStrip;

            toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStrip.AutoSize = true;

            trayIcon.MouseClick += TrayIcon_MouseClick;

            // Items
            toolStripLabel = new("");
            toolStripLabel.Enabled = false; // to gray it out
            toolStrip.Items.Add(toolStripLabel);

            ToolStripMenuItem setDurationItem;
            {
                ToolStripMenuItem item = new("Set Duration", null, NoAction);
                item.Tag = "";
                setDurationItem = item;
                toolStrip.Items.Add(item);
            }
            {
                ToolStripMenuItem item = new("Reset", null, Reset);
                item.Tag = "";
                item.Enabled = false;
                toolStripResetButton = item;
                //toolStrip.Items.Add(item);
            }
            {
                ToolStripMenuItem item = new("Exit", null, Exit);
                item.Tag = "";
                toolStrip.Items.Add(item);
            }


            // Set Duration items
            try
            {
                foreach (string _line in File.ReadAllLines(Program.DURATION_ITEMS_LIST_FILE))
                {
                    // preprocessing
                    string line = _line.Trim();

                    // dont consider lines that start with a comment
                    if (line.StartsWith("//"))
                    {
                        continue;
                    }

                    // remove trailing comments
                    int commentIndex = line.IndexOf("//");

                    if (commentIndex != -1)
                    {
                        line = line.Split("//")[0];
                        line = line.Trim();
                    }

                    // yea
                    if (line == String.Empty)
                    {
                        continue;
                    }

                    // parsing
                    float timeNumber = float.Parse(line) / 60.0f;
                    string timeString = TimeToReadableString(timeNumber);

                    // sanity checks
                    int timeNumberLimit = 1000000;
                    if (timeNumber <= 0 || timeNumber > timeNumberLimit)
                    {
                        throw new Exception($"Number must be positive and not exceed {timeNumberLimit}");
                    }

                    // create item
                    ToolStripMenuItem item = new(timeString, null, SetDuration);
                    item.Tag = timeNumber;
                    setDurationItem.DropDownItems.Add(item);
                    toolStripDurationButtonsList.Add(item);
                }
            }
            catch(Exception e)
            {
                Program.CrashLog($"Error reading \"{Program.DURATION_ITEMS_LIST_FILE}\"", "Either the file is missing or its contents are invalid.", e);
            }

            {
                ToolStripMenuItem item = new("None", null, SetDuration);
                item.Tag = -1.0f;
                setDurationItem.DropDownItems.Add(item);
                toolStripDurationButtonsList.Add(item);
            }
        }

        private void Reset(object? sender, EventArgs e)
        {
            OnReset();
        }

        private void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnClicked();
            }
        }

        void Exit(object? sender, EventArgs e)
        {
            OnExit();
        }

        void SetDuration(object? sender, EventArgs e)
        {
            foreach (var other in toolStripDurationButtonsList)
            {
                other.Checked = false;
            }

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = true;

            float time = (float)item.Tag; // we're expecting a float

            if( time == -1.0f )
            {
                OnSetNoDuration();
                return;
            }

            OnSetDuration(time);
        }

        void NoAction(object? sender, EventArgs e)
        {

        }

        string TimeToReadableString(float time)
        {
            TimeSpan ts = TimeSpan.FromHours(time);
            int iH = (int)Math.Floor(ts.TotalHours);
            int iM = (int)Math.Floor(ts.TotalMinutes % 60);
            int iS = (int)Math.Floor(ts.TotalSeconds % 60);

            string text;

            if (ts.TotalHours < 1)
            {
                if (ts.TotalMinutes >= 1)
                {
                    text = $"{iM} m";
                }
                else
                {
                    text = $"{iS} s";
                }
            }
            else
            {
                if (ts.TotalMinutes % 60 > 0)
                {
                    text = $"{iH} h, {iM} m";
                }
                else
                {
                    text = $"{iH} h";
                }
            }

            return text;
        }
    }
}
