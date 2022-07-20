using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Enum_GameEvent
{
    ToPos, KillEnemy, PickUp
}
/// <summary>
/// 监听所有的游戏事件（空，杀死敌人，拾取道具），并通知到对应的主题
/// </summary>
public class GameEventSystem : MonoBehaviour
{
    //一个事件类型对应一个主题
    static Dictionary<Enum_GameEvent, IGameEventSubject> _SubjectList = new Dictionary<Enum_GameEvent, IGameEventSubject>();
    /// <summary>
    /// 注册订阅者到对应的主题
    /// </summary>
    /// <param name="observer">订阅者</param>
    static public void Register(IGameEventObserver observer)
    {
        CheckSubjectListContain(observer.GetGameEvent());
        _SubjectList[observer.GetGameEvent()].Attach(observer);
    }
    /// <summary>
    /// 注销订阅者和主题
    /// </summary>
    /// <param name="observer">订阅者</param>
    static public void UnRegister(IGameEventObserver observer)
    {
        if (!_SubjectList.ContainsKey(observer.GetGameEvent())) return;
        _SubjectList[observer.GetGameEvent()].Detach(observer);
    }
    /// <summary>
    /// 通知对应的主题
    /// </summary>
    /// <param name="e">事件类型</param>
    /// <param name="para">事件信息</param>
    static public void Notify(Enum_GameEvent e, System.Object para)
    {
        if (!_SubjectList.ContainsKey(e)) { return; }
        _SubjectList[e].Notify(para);
    }

    static void CheckSubjectListContain(Enum_GameEvent e)
    {
        if (!_SubjectList.ContainsKey(e))
        {
            switch (e)
            {
                case Enum_GameEvent.KillEnemy:
                    _SubjectList.Add(e, new KillEnemySubject());
                    break;
                case Enum_GameEvent.PickUp:
                    _SubjectList.Add(e, new PickUpSubject());
                    break;
                case Enum_GameEvent.ToPos:
                    _SubjectList.Add(e, new ToPositionSubject());
                    break;
                default:
                    Debug.LogError("不包含该主题的设定：" + e);
                    break;
            }
        }
    }
    static public void Clear()
    {
        _SubjectList.Clear();
    }
}
