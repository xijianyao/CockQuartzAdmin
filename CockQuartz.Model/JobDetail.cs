
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
        public int Id { get; set; }

        [StringLength(50)]
        public string JobGroupName { get; set; }

        [StringLength(50)]
        public string JobName { get; set; }

        [StringLength(50)]
        public string TriggerName { get; set; }

        [StringLength(50)]
        public string TriggerGroupName { get; set; }

        [StringLength(50)]
        public string Cron { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        [StringLength(50)]
        public string CreateUser { get; set; }

        [StringLength(50)]
        public string UpdateUser { get; set; }

        [StringLength(200)]
        public string ExceptionEmail { get; set; }

        public bool IsDeleted { get; set; }
    }
}
