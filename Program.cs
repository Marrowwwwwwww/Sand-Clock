using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Resources;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SandClock
{
    internal static class Program
    {
        public static string DURATION_ITEMS_LIST_FILE = "interval_items_list.txt";
        static SandClockAppContext context;
        static Settings settings;
        static SandClock sandClock;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            IconResources.Init();
            settings = new();
            context = new(settings);
            sandClock = new(settings, context.GetTrayIcon());

            context.OnSetDuration = OnSetDuration;
            context.OnSetNoDuration = OnSetNoDuration;
            context.OnExit = OnExit;
            context.OnClicked = sandClock.Rotate;
            context.OnReset = sandClock.ResetProgress;

            context.Refresh();

            Application.Run(context);
        }

        public static void CrashLog(string message, string message2, Exception e)
        {
            MessageBox.Show(message2 + "\n\n" + "Error Log:\n" + e.Message.ToString(), message);
            Environment.Exit(0);
        }

        static void OnSetDuration(float value)
        {
            settings.Active = true;
            settings.flTimeInHours = value;
            sandClock.RefreshTimer();
            sandClock.ResetProgress();
            context.Refresh();
        }

        static void OnSetNoDuration()
        {
            settings.Active = false;
            sandClock.SetIdle();
            context.Refresh();
        }

        static void OnExit()
        {
            DialogResult result = MessageBox.Show("Stop the app?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            
            if (result == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }
    }

}