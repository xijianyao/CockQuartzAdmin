using CockQuartz.Model;

namespace CockQuartz.Interface
{
    public interface ICustomerJobInfoService
    {
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="jobGroupName">任务所在组名称</param>
        /// <param name="triggerName">触发器名称</param>
        /// <param name="triggerGroupName">触发器所在的组名称</param>
        /// <param name="cron">执行周期表达式</param>
        /// <param name="jobDescription">任务描述</param>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="exceptionEmail">job错误发送的邮箱</param>
        /// <returns>添加后任务编号</returns>
        int AddCustomerJobInfo(string jobName, string jobGroupName, string triggerName, string triggerGroupName, string cron, string jobDescription, string requestUrl, string exceptionEmail);

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="customerJobInfoModel"></param>
        /// <returns>更新的任务编号</returns>
        int UpdateCustomerJobInfo(JobDetail customerJobInfoModel);
    }
}
