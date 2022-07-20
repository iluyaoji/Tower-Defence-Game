
/// <summary>
/// 订阅者接口，主题和任务的中介
/// </summary>
public abstract class IGameEventObserver
{
    protected System.Action<System.Object> _action;
    
    public IGameEventObserver(System.Action<System.Object> action)
    {
        _action = action;
    }
    public void Register()
    {
        GameEventSystem.Register(this);
    }
    public void UnRegister()
    {
        GameEventSystem.UnRegister(this);
    }
    /// <summary>
    /// 返回订阅的主题类型
    /// </summary>
    /// <returns></returns>
    public abstract Enum_GameEvent GetGameEvent();
    /// <summary>
    /// 更新任务信息
    /// </summary>
    public void Update(System.Object para)
    {
        _action(para);
    }
}
