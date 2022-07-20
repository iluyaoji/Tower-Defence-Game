using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 任务管理器，负责将任务信息显示到UI上面
/// </summary>
public class TaskManager : MonoBehaviour
{

    //任务UI
    //public TaskTipUI TaskTipUIRef;
    public List<TaskContext> taskList;
    public Transform TaskRoot;
    //任务列表
    Dictionary<ITask,bool> _Tasks = new Dictionary<ITask, bool>();
    //当前执行的任务
    private ITask CurrentTaskID = null;
    bool _doingTask;

    string _target;
    string _content;

    private void Awake()
    {
        foreach(var v in taskList){
            if(v.taskType == ENUME_TaskType.Kill)
            {
                Task_KillEnemy task = new Task_KillEnemy(this,v);
                AddTask(task);
                task.StartTask();
            }
        }
    }
    public void Initial()
    {
        UIMenu o = FindObjectOfType<UIMenu>();
        if (!o) return;

        TaskRoot = o.transform.Find("TaskView").GetChild(0).GetChild(0);
        if (!TaskRoot)
        {
            Debug.Log("没有找到任务列表");
            return;
        }
        //Debug.Log("TaskManager Start");
        int current = 0;
        foreach (var task in _Tasks)
        {
            if (current >= TaskRoot.childCount)
            {
                Instantiate(TaskRoot.GetChild(current - 1).gameObject, TaskRoot.GetChild(current - 1).parent);
            }
            TaskRoot.GetChild(current++).GetComponent<TaskTip>().context = task.Key.context;
        }
    }

    public bool GetTaskState(ITask ID)
    {
        return _Tasks[ID];
    }
    /// <summary>
    /// 设置任务显示UI
    /// </summary>
    /// <param name="target">UI标识</param>
    /// <param name="content">内容</param>
    public void SetUpdateUI(string target, string content)
    {
        _target = target;
        _content = content;
    }
    /// <summary>
    /// 更新UI
    /// </summary>
    public void UpdataTaskUI()
    {
        if (_doingTask)
        {
            //TaskTipUIRef.UpdataTip(_target, _content);
        }
    }
    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="task">任务对象</param>
    /// <param name="finishiTaskCallBack">完成回调</param>
    public void AddTask(ITask task, Action<TaskContext> finishiTaskCallBack = null)
    {
        task._FinishTaskCallBack = finishiTaskCallBack;
        _Tasks.Add(task,false);
    }
    /// <summary>
    /// 开始任务
    /// </summary>
    public void StartTask(ITask ID)
    {
        if (_Tasks.Count == 0 || ID == null || _Tasks[ID]) return;
        //TaskTipUIRef.gameObject.SetActive(true);
        _doingTask = true;
        CurrentTaskID = ID;
        ID.StartTask();
    }
    public void StartTask()
    {
        StartTask(CurrentTaskID);
    }
    /// <summary>
    /// 任务完成后的事件
    /// </summary>
    /// <param name="ID">该任务</param>
    public void DoAfterTaskFinish(ITask ID)
    {
        _Tasks[ID] = true;
        if (ID._FinishTaskCallBack != null)
            ID._FinishTaskCallBack.Invoke(ID.context);
        ID._FinishTaskCallBack = null;
        // 注销任务的订阅者
        StartCoroutine("DelayKillObserver", ID);
    }
    /// <summary>
    /// 延迟注销任务的订阅者
    /// </summary>
    /// <param name="task">将注销任务</param>
    /// <returns></returns>
    IEnumerator DelayKillObserver(ITask task)
    {
        yield return new WaitForSecondsRealtime(0);
        task.KillObserver();
    }

    public void SubmitTask(ITask ID)
    {
        if (!_doingTask) return;
        //TaskTipUIRef.gameObject.SetActive(false);
        ID.OnSubmitTask();
        _doingTask = false;
    }
}
