
using System;
/// <summary>
/// PickUp主题订阅者
/// </summary>
public class PickUpObserver : IGameEventObserver
{
    public PickUpObserver(Action<object> action) : base(action)
    {
    }

    public override Enum_GameEvent GetGameEvent()
    {
        return Enum_GameEvent.PickUp;
    }
}
