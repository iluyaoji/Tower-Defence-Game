using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 主题接口，数据类，包含该主题统计和所有的影响该主题的订阅者
/// </summary>
public abstract class IGameEventSubject
{
    List<IGameEventObserver> _observerList = new List<IGameEventObserver>();
    
    /// <summary>
    /// 添加订阅者
    /// </summary>
    /// <param name="observer"></param>
    public void Attach(IGameEventObserver observer)
    {
        _observerList.Add(observer);
    }
    /// <summary>
    /// 注销订阅者
    /// </summary>
    /// <param name="observer"></param>
    public void Detach(IGameEventObserver observer)
    {
        _observerList.Remove(observer);
    }
    /// <summary>
    /// 更新主题数据统计，在通知所有订阅者之前调用
    /// </summary>
    /// <param name="para">事件信息</param>
    protected abstract void BeforeNotifyObserver(System.Object para);
    /// <summary>
    /// 通知所有的订阅者，主题信息更新
    /// </summary>
    /// <param name="para">主题关键</param>
    public virtual void Notify(System.Object para)
    {
        BeforeNotifyObserver(para);
        foreach (var item in _observerList)
        {
            item.Update(para);
        }
    }
}
