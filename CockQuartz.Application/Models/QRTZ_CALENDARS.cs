using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CockQuartz.Core.Models
{
    public partial class QRTZ_CALENDARS
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string SCHED_NAME { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(200)]
        public string CALENDAR_NAME { get; set; }

        [Column(TypeName = "image")]
        [Required]
        public byte[] CALENDAR { get; set; }
    }
}
