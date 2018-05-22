using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eHi.Job.Core.Models
{
    public partial class QRTZ_TRIGGERS
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string SCHED_NAME { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(150)]
        public string TRIGGER_NAME { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string TRIGGER_GROUP { get; set; }

        [Required]
        [StringLength(150)]
        public string JOB_NAME { get; set; }

        [Required]
        [StringLength(150)]
        public string JOB_GROUP { get; set; }

        [StringLength(250)]
        public string DESCRIPTION { get; set; }

        public long? NEXT_FIRE_TIME { get; set; }

        public long? PREV_FIRE_TIME { get; set; }

        public int? PRIORITY { get; set; }

        [Required]
        [StringLength(16)]
        public string TRIGGER_STATE { get; set; }

        [Required]
        [StringLength(8)]
        public string TRIGGER_TYPE { get; set; }

        public long START_TIME { get; set; }

        public long? END_TIME { get; set; }

        [StringLength(200)]
        public string CALENDAR_NAME { get; set; }

        public int? MISFIRE_INSTR { get; set; }

        [Column(TypeName = "image")]
        public byte[] JOB_DATA { get; set; }

        public virtual QRTZ_CRON_TRIGGERS QRTZ_CRON_TRIGGERS { get; set; }

        public virtual QRTZ_JOB_DETAILS QRTZ_JOB_DETAILS { get; set; }

        public virtual QRTZ_SIMPLE_TRIGGERS QRTZ_SIMPLE_TRIGGERS { get; set; }

        public virtual QRTZ_SIMPROP_TRIGGERS QRTZ_SIMPROP_TRIGGERS { get; set; }
    }
}
