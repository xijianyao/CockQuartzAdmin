using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eHi.Job.Core.Models
{
    public partial class QRTZ_SIMPROP_TRIGGERS
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

        [StringLength(512)]
        public string STR_PROP_1 { get; set; }

        [StringLength(512)]
        public string STR_PROP_2 { get; set; }

        [StringLength(512)]
        public string STR_PROP_3 { get; set; }

        public int? INT_PROP_1 { get; set; }

        public int? INT_PROP_2 { get; set; }

        public long? LONG_PROP_1 { get; set; }

        public long? LONG_PROP_2 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DEC_PROP_1 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? DEC_PROP_2 { get; set; }

        public bool? BOOL_PROP_1 { get; set; }

        public bool? BOOL_PROP_2 { get; set; }

        public virtual QRTZ_TRIGGERS QRTZ_TRIGGERS { get; set; }
    }
}
