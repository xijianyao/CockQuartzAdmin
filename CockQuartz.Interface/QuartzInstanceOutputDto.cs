using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockQuartz.Interface
{
    public class QuartzInstanceOutputDto
    {
        public string InstanceName { get; set; }

        public DateTime LastCheckInTime { get; set; }
    }
}
