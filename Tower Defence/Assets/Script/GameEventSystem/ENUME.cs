
/// <summary>
/// 敌人枚举
/// </summary>
public enum ENUME_EnemyType
{
    None,
    Cube,
    Sphere
}

/// <summary>
/// 拾取类型枚举
/// </summary>
public enum ENUME_PickUpType
{
    None,
    Gift, //礼物
    Other
}

/// <summary>
/// 初始位置类型枚举
/// </summary>
public enum ENUME_StartPoint
{
    Default,
    Gate, //大门
}
public enum ENUME_TaskType
{
    None,
    ToPos,
    PickUp,
    Kill,
}