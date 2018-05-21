using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CockQuartz.Core.Models;
using DapperWrapper;
using EHiDBConfigNew.EhiConfig;

namespace CockQuartz.Core.JobManager
{
    public class JobMangerDal
    {
        private readonly IDbExecutorFactory _dbExecutorFactory;
        private readonly string _connectString;
        public static readonly string Environment = ConfigurationManager.AppSettings["Environment"];
        public static readonly string ConfigKey = ConfigurationManager.ConnectionStrings["EhiJob"].ConnectionString;

        #region 连接字符串

        private static string GetDbConfigUrl()
        {
            switch (Environment)
            {
                case "prod":
                case "production":
                    return "http://dbcfgnewapi.1hai.cn/dbcfgAPI/ConfigService/";
                case "demo":
                    return "http://demoapi.1hai.cn/DbConfigNewService/ConfigService/";
                case "dev":
                    return "http://192.168.1.229:8021/ConfigService/";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static string GetConString()
        {
            string con = "";
            string errorMsg = "";
            DBInfo dbInfo = DBConfig.GetConfig(GetDbConfigUrl(), ConfigKey, "EhiJob", out errorMsg);
            if (dbInfo != null)
                con = "Data Source=" + dbInfo.ServerName + ";Initial Catalog=" + dbInfo.DbName + ";User ID=" + dbInfo.LoginName + ";Password=" + dbInfo.LoginPwd + ";Connect Timeout=300000";
            return con;

            //return "Data Source=192.168.9.51;Initial Catalog=EhiJob;User ID=Dev_GW_FB;Password=rIbfWhMVQo$Z+II1TIHJ;Connect Timeout=300000";
        }
        #endregion

        public JobMangerDal()
        {
            _dbExecutorFactory = new SqlExecutorFactory();
            _connectString = GetConString();
        }

        public List<JobDetail> GetJobDetails()
        {
            List<JobDetail> result;
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = "select * from jobdetail where isdeleted = 0";
                result = dbExecuter.Query<JobDetail>(sql).ToList();
            }
            return result;
        }

        public List<JobDetail> GetJobDetailsByGroupName(string groupName)
        {
            List<JobDetail> result;
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = "select * from jobdetail where JobGroupName = @GroupName and isdeleted = 0";
                result = dbExecuter.Query<JobDetail>(sql, new { GroupName = groupName }).ToList();
            }
            return result;
        }

        public JobDetail GetJobDetailsById(int id)
        {
            JobDetail result;
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = "select * from jobdetail where id= @Id and isdeleted = 0";
                result = dbExecuter.Query<JobDetail>(sql, new { Id = id }).FirstOrDefault();
            }
            return result;
        }

        public int InsertJobDetailAndGetId(JobDetail jobDetail)
        {
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string isExistJobSql =
                    @"select count(1) from [dbo].[JobDetail](nolock) where JobGroupName = @JobGroupName and JobName = @JobName and isdeleted = 0";
                if (dbExecuter.Query<int>(isExistJobSql, jobDetail).First() > 0)
                {
                    throw new Exception("已经存在该job，无法添加");
                }

                string sql = @"
INSERT INTO [dbo].[JobDetail]
           ([JobGroupName],[JobName],[TriggerName],[TriggerGroupName],[Cron],[Description],[CreateTime]
           ,[UpdateTime],[CreateUser],[UpdateUser],[ExceptionEmail],[IsDeleted])
     VALUES
           (@JobGroupName,@JobName,@TriggerName,@TriggerGroupName,@Cron,@Description
           ,@CreateTime,@UpdateTime,@CreateUser,@UpdateUser,@ExceptionEmail,@IsDeleted) 
select @@identity 
";
                return dbExecuter.Query<int>(sql, jobDetail).First();
            }
        }

        public void UpdateJobDetail(JobDetail jobDetail)
        {
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = @"UPDATE [dbo].[JobDetail]
   SET [JobGroupName] = @JobGroupName,[JobName] = @JobName,[TriggerName] = @TriggerName,[TriggerGroupName] = @TriggerGroupName
      ,[Cron] = @Cron,[Description] = @Description,[CreateTime] = @CreateTime,[UpdateTime] = @UpdateTime,[CreateUser] = @CreateUser
      ,[UpdateUser] = @UpdateUser,[ExceptionEmail] = @ExceptionEmail,[IsDeleted] = @IsDeleted
 WHERE Id = @Id";
                dbExecuter.Execute(sql, jobDetail);
            }
        }

        public void DeleteJobDetail(int id)
        {
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = "update jobdetail set isdeleted = 1 where id = @Id";
                dbExecuter.Execute(sql, new { Id = id });
            }
        }

        public List<JobExecuteLogs> GetJobExecuteLogsByJobId(int id)
        {
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = "select top 30 * from JobExecuteLogs(nolock) where JobDetailId = @Id";
                return dbExecuter.Query<JobExecuteLogs>(sql, new { Id = id }).ToList();
            }
        }

        public int InsertJobExecuteLogs(JobExecuteLogs jobExecuteLogs)
        {
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = @"
INSERT INTO [dbo].[JobExecuteLogs]([JobDetailId],[JobGroupName],[JobName],[IsSuccess],[ExecuteInstanceName]
           ,[ExecuteInstanceIp],[Message],[ExceptionMessage],[CreationTime],[Duration])
     VALUES
           (@JobDetailId,@JobGroupName,@JobName,@IsSuccess,@ExecuteInstanceName
           ,@ExecuteInstanceIp,@Message,@ExceptionMessage,@CreationTime,@Duration) 
select @@identity ";
                return dbExecuter.Query<int>(sql, jobExecuteLogs).First();
            }

        }

        public void UpdateJobExecuteLogs(JobExecuteLogs jobExecuteLogs)
        {
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = @"
UPDATE [dbo].[JobExecuteLogs]
   SET [JobDetailId] = @JobDetailId
      ,[JobGroupName] = @JobGroupName
      ,[JobName] = @JobName
      ,[IsSuccess] = @IsSuccess
      ,[ExecuteInstanceName] = @ExecuteInstanceName
      ,[ExecuteInstanceIp] = @ExecuteInstanceIp
      ,[Message] = @Message
      ,[ExceptionMessage] = @ExceptionMessage
      ,[CreationTime] = @CreationTime
      ,[Duration] = @Duration
 WHERE Id = @Id
";
                dbExecuter.Execute(sql, jobExecuteLogs);
            }

        }


        public List<QRTZ_SCHEDULER_STATE> GetSchedulerState(string schedName)
        {
            using (var dbExecuter = _dbExecutorFactory.CreateExecutor(_connectString))
            {
                string sql = "select * from  QRTZ_SCHEDULER_STATE where SCHED_NAME = @SchedName";
                return dbExecuter.Query<QRTZ_SCHEDULER_STATE>(sql, new { SchedName = schedName }).ToList();
            }
        }
    }
}
