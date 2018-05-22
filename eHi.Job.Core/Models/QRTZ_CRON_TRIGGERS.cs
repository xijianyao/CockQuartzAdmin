using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eHi.Job.Core.Models
{
    public partial class QRTZ_CRON_TRIGGERS
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
        [StringLength(120)]
        public string CRON_EXPRESSION { get; set; }

        [StringLength(80)]
        public string TIME_ZONE_ID { get; set; }

        public virtual QRTZ_TRIGGERS QRTZ_TRIGGERS { get; set; }
    }
}
