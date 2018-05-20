using System;

namespace CockQuartz.Core.Dto
{
    public class JobDetailOutputDto
    {
        public int Id { get; set; }

        public string JobGroupName { get; set; }

        public string JobName { get; set; }

        public string TriggerName { get; set; }

        public string TriggerGroupName { get; set; }

        public string Cron { get; set; }

        public string Description { get; set; }

        public string CurrentStatus { get; set; }

        public string NextRunTime { get; set; }

        public string LastRunTime { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

        public string ExceptionEmail { get; set; }

        public bool IsInSchedule { get; set; }

        public bool IsDeleted { get; set; }
    }
}
