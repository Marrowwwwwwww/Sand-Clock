using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandClock
{
    internal class SandClock
    {
        private System.Windows.Forms.Timer timer;
        private NotifyIcon trayIcon;
        private float progress = 0;
        private Settings settings;

        public SandClock(Settings settings, NotifyIcon trayIcon)
        {
            this.settings = settings;
            this.trayIcon = trayIcon;

            timer = new();
            timer.Tick += OnTimer;
            SetIdle();
        }

        public void OnTimer(object? sender, EventArgs e)
        {
            progress += (1.0f / (float)IconResources.NUM_PROGRESS_ICONS);

            if (progress > 1)
            {
                timer.Stop();
            }

            progress = Math.Clamp(progress, 0.0f, 1.0f);

            UpdateIcon();
        }

        public void Rotate()
        {
            if (!settings.Active)
            {
                return;
            }

            progress = 1.0f - progress;
            RefreshTimer();
            UpdateIcon();
        }

        public void RefreshTimer()
        {
            timer.Interval = (int)((settings.flTimeInHours / (float)IconResources.NUM_PROGRESS_ICONS) * 60 * 60 * 1000);
            timer.Stop();
            timer.Start();
            UpdateIcon();
        }

        public void ResetProgress()
        {
            progress = 0;
            UpdateIcon();
        }

        public void SetIdle()
        {
            trayIcon.Icon = IconResources.GetDefaultIcon();
            timer.Stop();
        }

        public void UpdateIcon()
        {
            trayIcon.Icon = IconResources.GetProgressIcon(progress);
        }
    }
}
