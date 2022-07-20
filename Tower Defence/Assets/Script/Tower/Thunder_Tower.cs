using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder_Tower : ITower
{
    public float BulletCost;
    public override void BeforeAttack()
    {
        if (gameManager.Level_Money - BulletCost < 0)
            isAttack = false;
    }
    public override void AfterAttack()
    {
        gameManager.Level_Money -= BulletCost;
    }

}
