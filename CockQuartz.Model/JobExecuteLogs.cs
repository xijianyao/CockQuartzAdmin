using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CockQuartz.Model
{
    [Table("JobExecuteLogs")]
    public class JobExecuteLogs 
    {
        [Index("INDEX_REGNUM")]
        public int JobDetailId { get; set; }

        [StringLength(50)]
        public string JobGroupName { get; set; }

        [StringLength(50)]
        public string JobName { get; set; }

        public bool IsSuccess { get; set; }

        [StringLength(500)]
        public string ExecuteInstanceName { get; set; }

        [StringLength(200)]
        public string ExecuteInstanceIp { get; set; }

        [StringLength(500)]
        public string Message { get; set; }

        [StringLength(500)]
        public string ExceptionMessage { get; set; }

        [StringLength(5000)]
        public string ExceptionStack { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
