using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SandClock
{

    internal class Settings
    {
        public float flTimeInHours { get; set; }
        public bool Active { get; set; }

        public Settings()
        {
            this.flTimeInHours = 0.0f;
            this.Active = false;
        }
    }

}
