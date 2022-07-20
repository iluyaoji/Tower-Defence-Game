using System;
/// <summary>
/// 杀敌主题订阅者
/// </summary>
public class KillEnemyObserver : IGameEventObserver
{
    public KillEnemyObserver(Action<object> action) : base(action)
    {
    }

    public override Enum_GameEvent GetGameEvent()
    {
        return Enum_GameEvent.KillEnemy;
    }

}