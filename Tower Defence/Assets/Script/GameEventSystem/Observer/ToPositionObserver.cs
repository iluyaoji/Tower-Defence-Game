
using System;
/// <summary>
/// KillEnemy主题订阅者
/// </summary>
public class ToPositionObserver : IGameEventObserver
{
    public ToPositionObserver(Action<object> action) : base(action)
    {
    }

    public override Enum_GameEvent GetGameEvent()
    {
        return Enum_GameEvent.ToPos;
    }

}
