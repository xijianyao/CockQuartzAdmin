namespace CockQuartz.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class QRTZ_JOB_DETAILS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QRTZ_JOB_DETAILS()
        {
            QRTZ_TRIGGERS = new HashSet<QRTZ_TRIGGERS>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string SCHED_NAME { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(150)]
        public string JOB_NAME { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string JOB_GROUP { get; set; }

        [StringLength(250)]
        public string DESCRIPTION { get; set; }

        [Required]
        [StringLength(250)]
        public string JOB_CLASS_NAME { get; set; }

        public bool IS_DURABLE { get; set; }

        public bool IS_NONCONCURRENT { get; set; }

        public bool IS_UPDATE_DATA { get; set; }

        public bool REQUESTS_RECOVERY { get; set; }

        [Column(TypeName = "image")]
        public byte[] JOB_DATA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QRTZ_TRIGGERS> QRTZ_TRIGGERS { get; set; }
    }
}
