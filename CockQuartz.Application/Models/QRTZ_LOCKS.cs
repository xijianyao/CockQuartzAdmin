using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CockQuartz.Core.Models
{
    public partial class QRTZ_LOCKS
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string SCHED_NAME { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string LOCK_NAME { get; set; }
    }
}
