using System;

namespace CockQuartz.Core
{
    public class QuartzInstanceOutputDto
    {
        public string InstanceName { get; set; }

        public DateTime LastCheckInTime { get; set; }

        public string HeartBeat { get; set; }
    }
}
