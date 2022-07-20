using UnityEngine;
/// <summary>
/// 杀敌统计主题
/// </summary>
public class KillEnemySubject : IGameEventSubject
{
    public int _killNum = 0;
    protected override void BeforeNotifyObserver(object para)
    {
        if (para.GetType() == typeof(ENUME_EnemyType))
        {
            _killNum++;
            Debug.Log(_killNum);
        }
    }
}
