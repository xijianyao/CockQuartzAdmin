namespace CockQuartz.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class QRTZ_SCHEDULER_STATE
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string SCHED_NAME { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(200)]
        public string INSTANCE_NAME { get; set; }

        public long LAST_CHECKIN_TIME { get; set; }

        public long CHECKIN_INTERVAL { get; set; }
    }
}
