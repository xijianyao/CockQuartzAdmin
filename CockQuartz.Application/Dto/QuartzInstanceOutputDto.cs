using System;

namespace CockQuartz.Core.Dto
{
    public class QuartzInstanceOutputDto
    {
        public string InstanceName { get; set; }

        public DateTime LastCheckInTime { get; set; }

        public string HeartBeat { get; set; }
    }
}
