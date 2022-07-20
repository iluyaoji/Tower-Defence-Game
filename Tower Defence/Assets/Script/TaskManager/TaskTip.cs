using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTip : MonoBehaviour
{
    public Text text;
    public bool isDone;
    public TaskContext context;

    private void Update()
    {
        text.text = context.ToString();
    }
}
[System.Serializable]
public class TaskContext
{
    public string Name;
    public string Description;
    public ENUME_TaskType taskType;
    public ENUME_EnemyType enemyType;
    public int Num;

    public override string ToString()
    {
        return Name + "--" + Num;
    }
}
