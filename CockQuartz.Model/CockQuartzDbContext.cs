using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using CockQuartz.Model.Repository;
using EntityFramework.DynamicFilters;
using FeI.EntityFramework;
using FeI.EntityFramework.Reporitory;

namespace CockQuartz.Model
{
    [RepositoryTypes(typeof(MainRepository<>), typeof(MainRepository<,>))]
    public partial class CockQuartzDbContext : FeIDbContext
    {
        public CockQuartzDbContext()
            : base("name=CockQuartz")
        {
        }

        public virtual DbSet<JobDetail> JobDetail { get; set; }
        public virtual DbSet<QRTZ_BLOB_TRIGGERS> QRTZ_BLOB_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_CALENDARS> QRTZ_CALENDARS { get; set; }
        public virtual DbSet<QRTZ_CRON_TRIGGERS> QRTZ_CRON_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_FIRED_TRIGGERS> QRTZ_FIRED_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_JOB_DETAILS> QRTZ_JOB_DETAILS { get; set; }
        public virtual DbSet<QRTZ_LOCKS> QRTZ_LOCKS { get; set; }
        public virtual DbSet<QRTZ_PAUSED_TRIGGER_GRPS> QRTZ_PAUSED_TRIGGER_GRPS { get; set; }
        public virtual DbSet<QRTZ_SCHEDULER_STATE> QRTZ_SCHEDULER_STATE { get; set; }
        public virtual DbSet<QRTZ_SIMPLE_TRIGGERS> QRTZ_SIMPLE_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_SIMPROP_TRIGGERS> QRTZ_SIMPROP_TRIGGERS { get; set; }
        public virtual DbSet<QRTZ_TRIGGERS> QRTZ_TRIGGERS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobDetail>()
                .HasKey(e => e.Id)
                .Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<QRTZ_JOB_DETAILS>()
                .HasMany(e => e.QRTZ_TRIGGERS)
                .WithRequired(e => e.QRTZ_JOB_DETAILS)
                .HasForeignKey(e => new { e.SCHED_NAME, e.JOB_NAME, e.JOB_GROUP })
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<QRTZ_SIMPROP_TRIGGERS>()
                .Property(e => e.DEC_PROP_1)
                .HasPrecision(13, 4);

            modelBuilder.Entity<QRTZ_SIMPROP_TRIGGERS>()
                .Property(e => e.DEC_PROP_2)
                .HasPrecision(13, 4);

            modelBuilder.Entity<QRTZ_TRIGGERS>()
                .HasOptional(e => e.QRTZ_CRON_TRIGGERS)
                .WithRequired(e => e.QRTZ_TRIGGERS)
                .WillCascadeOnDelete();

            modelBuilder.Entity<QRTZ_TRIGGERS>()
                .HasOptional(e => e.QRTZ_SIMPLE_TRIGGERS)
                .WithRequired(e => e.QRTZ_TRIGGERS)
                .WillCascadeOnDelete();

            modelBuilder.Entity<QRTZ_TRIGGERS>()
                .HasOptional(e => e.QRTZ_SIMPROP_TRIGGERS)
                .WithRequired(e => e.QRTZ_TRIGGERS)
                .WillCascadeOnDelete();

            //modelBuilder.DisableFilterGlobally("SoftDelete");
        }
    }
}
