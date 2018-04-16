using System;

namespace CockQuartz.Interface
{
    public class QuartzInstanceOutputDto
    {
        public string InstanceName { get; set; }

        public DateTime LastCheckInTime { get; set; }

        public string HeartBeat { get; set; }
    }
}
