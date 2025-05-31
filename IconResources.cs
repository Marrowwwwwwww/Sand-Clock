using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandClock
{

    internal static class IconResources
    {
        public static int NUM_PROGRESS_ICONS = 32;
        static List<Icon> progressIcons = new();
        static Icon defaultIcon;

        public static void Init()
        {
            try
            {
                LoadIcons();
            }
            catch(Exception e)
            {
                Program.CrashLog("Couldn't load icons", "", e);
            }
        }

        public static Icon GetProgressIcon(float value)
        {
            value = ProgressIconSamplingCurve(value);
            value = Math.Clamp(value, 0.0f, 1.0f);
            int id = (int)
                Math.Round(value * (float)(progressIcons.Count - 1));

            return progressIcons[id];
        }

        public static Icon GetDefaultIcon()
        {
            return defaultIcon;
        }

        private static float ProgressIconSamplingCurve(float value)
        {
            float linear = value;
            float curve = 1.0f - (float)Math.Sqrt(1.0f - Math.Pow(value, 2.0f));

            float w = 0.6f; // curve weight
            // mix curve and linear by the weight
            return (curve * w) + (linear * (1.0f - w));
        }

        private static void LoadIcons()
        {
            for (int i = 0; i < NUM_PROGRESS_ICONS; i++)
            {
                progressIcons.Add(
                    new Icon("ico/sandclock" + i.ToString("0000") + ".ico"));
            }

            defaultIcon = new Icon("ico/rotated.ico");
        }
    }

}
