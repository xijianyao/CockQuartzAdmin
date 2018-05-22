using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eHi.Job.Core.Models
{
    public partial class QRTZ_PAUSED_TRIGGER_GRPS
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string SCHED_NAME { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(150)]
        public string TRIGGER_GROUP { get; set; }
    }
}
