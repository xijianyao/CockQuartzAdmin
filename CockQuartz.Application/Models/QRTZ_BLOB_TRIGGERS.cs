using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CockQuartz.Core.Models
{
    public partial class QRTZ_BLOB_TRIGGERS
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

        [Column(TypeName = "image")]
        public byte[] BLOB_DATA { get; set; }
    }
}
