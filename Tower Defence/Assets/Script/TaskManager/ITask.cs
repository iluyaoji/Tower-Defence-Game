using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 任务接口
/// </summary>
public abstract class ITask 
{
    /// <summary>
    /// 该任务的管理器
    /// </summary>
    protected TaskManager _manager;
    /// <summary>
    /// 订阅者（任务监视器）
    /// </summary>
    protected IGameEventObserver _observer;
    /// <summary>
    /// 完成任务回调
    /// </summary>
    public System.Action<TaskContext> _FinishTaskCallBack;
    /// <summary>
    /// 任务内容
    /// </summary>
    public TaskContext context;
    /// <summary>
    /// 任务依赖一个任务管理器
    /// </summary>
    /// <param name="manager">任务管理器</param>
    public ITask(TaskManager manager)
    {
        _manager = manager;
    }
    /// <summary>
    /// 初始化任务，开始任务之前调用
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// 开始任务
    /// </summary>
    public virtual void StartTask()
    {
        if(_observer == null)
        {
            Start();
            if (_observer == null)
                Debug.LogError("_observer未被初始化");
        }
        else
        {
            Updata(null);
        }
    }
    /// <summary>
    /// 任务内容，当事件监听系统监听到相应的主题事件时被调用
    /// </summary>
    /// <param name="para">待确认的主题关键（信息），检查是否符合任务要求</param>
    public abstract void Updata(System.Object para);
    /// <summary>
    /// 任务完成
    /// </summary>
    public abstract void OnTaskFinish();
    /// <summary>
    /// 注销订阅者
    /// </summary>
    public abstract void KillObserver();
    /// <summary>
    /// 提交任务
    /// </summary>
    public abstract void OnSubmitTask();
}
