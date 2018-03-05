namespace CockQuartz.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("JobDetail")]
    public partial class JobDetail
    {
        public Guid Id { get; set; }

        [StringLength(50)]
        public string GroupName { get; set; }

        [StringLength(50)]
        public string JobName { get; set; }

        [StringLength(50)]
        public string TriggerName { get; set; }

        [StringLength(50)]
        public string Cron { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
