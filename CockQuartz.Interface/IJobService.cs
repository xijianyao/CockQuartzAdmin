﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CockQuartz.Model;
using eHi.Common.Dto.Paged;

namespace CockQuartz.Interface
{
    public interface IJobService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        int CreateJob(JobDetail job);

        /// <summary>
        /// /获取任务列表
        /// </summary>
        /// <returns></returns>
        PagedResultDto<JobDetailOutputDto> GetJobList(int pageIndex , string groupName="");

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="id">任务信息</param>
        /// <returns></returns>
        bool RunJob(int id);

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="id">任务信息</param>
        /// <returns></returns>

        bool DeleteJob(int id);

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="id">任务信息</param>
        /// <returns></returns>
        bool PauseJob(int id);

        /// <summary>
        /// 恢复任务
        /// </summary>        
        /// <returns></returns>
        bool ResumeJob(int id);

        /// <summary>
        /// 更改任务运行周期
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        bool ModifyJobCron(int id, string cron);

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool StartJob(int id);

        /// <summary>
        /// 获取运行的实例
        /// </summary>
        /// <returns></returns>
        List<QuartzInstanceOutputDto> GetQuartzInstances();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="exceptionEmail"></param>
        /// <returns></returns>
        bool ModifyExceptionEmail(int id, string exceptionEmail);

        bool ModifyRequestUrl(int id, string requestUrl);

        List<JobExecuteLogs> GetJobLogList(int jobId);
    }
}
