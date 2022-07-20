using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_KillEnemy : ITask
{
    int _totalNum;
    int _killNum = 0;
    public Task_KillEnemy(TaskManager manager, TaskContext context) : base(manager)
    {
        this.context = context;
        _totalNum = context.Num;
    }

    public override void KillObserver()
    {
        _observer.UnRegister();
    }

    public override void OnSubmitTask()
    {
        
    }

    public override void OnTaskFinish()
    {
        _manager.SetUpdateUI(context.Name, "完成");
        _manager.UpdataTaskUI();
        _manager.DoAfterTaskFinish(this);
    }

    public override void Start()
    {
        _observer = new KillEnemyObserver(Updata);
        _observer.Register();
        _manager.SetUpdateUI(context.Name, context.Description + _totalNum + "/" + _killNum);  //更新任务UI
        _manager.UpdataTaskUI();
    }

    public override void Updata(object para)
    {
        if (para == null)
        {
            _manager.SetUpdateUI(context.Name, context.Description + _totalNum + "/" + _killNum);
            _manager.UpdataTaskUI();
            return;
        }
        ENUME_EnemyType t = (ENUME_EnemyType)para;
        if (t == context.enemyType)
        {
            //int num = (int)para;
            _killNum++;
            _manager.SetUpdateUI(context.Name, context.Description + _totalNum +"/"+ _killNum);
            _manager.UpdataTaskUI();
            //如果消灭花妖达到任务目标，则调用任务完成
            if (_killNum >= _totalNum)
            {
                context.Name = context.Name + "(完成)";
                OnTaskFinish();
            }
        }
    }
}
