using System.ComponentModel;

namespace eHi.Job.Core.Dto
{
    /// <summary>
    /// 执行状态
    /// </summary>
    public enum ExecuteStatusType
    {
        [Description("待执行")]
        WaitExecute = 0,
        [Description("执行中")]
        Executing = 1
    }
}
