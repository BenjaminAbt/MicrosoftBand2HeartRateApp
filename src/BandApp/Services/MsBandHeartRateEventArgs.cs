using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandApp.Services
{
    public class MsBandHeartRateEventArgs : EventArgs
    {
        public int HeartRate { get; }

        public MsBandHeartRateEventArgs( int heartRate )
        {
            HeartRate = heartRate;
        }
    }
}
