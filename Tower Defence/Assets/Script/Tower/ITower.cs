using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ITower : MonoBehaviour
{
    public float price;
    public int Lv = 1;
    public float ATK = 1;
    //[SerializeField] GameObject ClickUI;
    //火力点的部位
    public Transform Head;
    public float FireField = 1;
    //火力范围可视贴图
    public Transform FireFieldView;
    public float buildDelay = 1;
    //建造的起始点
    public Transform completeStart;
    //建造的结束点
    public Transform completeEnd;
    //子弹预制体
    public GameObject _Bullet;
    [Range(1, 10)]
    public float BulletVelocity;
    //火力点
    public Transform[] FirePoints;
    [Range(1, 10)]
    public float FireRate = 1.0f;
    protected Transform BulletPool;
    protected Transform EnemyPool;

    protected float degreeOfCompletion = 0;
    protected bool complete = false;
    protected Transform fireGoal;
    protected bool isAttack = false;
    protected List<Transform> allChildren;
    protected LevelManager gameManager;

    private void Awake()
    {
        if (FireFieldView != null)
            FireFieldView.localScale = new Vector3(2 * FireField * 10, 2, 2 * FireField * 10);
    }
    void Start()
    {
        if (Head == null) Head = transform;
        gameManager = LevelManager.Instance;
        if (gameManager != null)
        {
            BulletPool = gameManager.BulletPool;
            EnemyPool = gameManager.EnemyPool;
        }
        else if (BulletPool == null)
            BulletPool = transform;
        StartCoroutine(Attack());
        allChildren = LevelManager.FindAllChildren(transform);

    }

    void Update()
    {
        if (complete)
            Aim();
        else
            Building();
    }
    /// <summary>
    /// 建造函数
    /// </summary>
    /// <returns></returns>
    public float Building()
    {
        if (completeEnd == null || completeStart == null)
        {
            complete = true;
            return 0;
        }
        //建造持续时间
        degreeOfCompletion += Time.deltaTime;
        //建造百分比
        float value = degreeOfCompletion / buildDelay;
        //给炮塔所有的材质传递建造完成度
        foreach (Transform t in allChildren)
        {
            if (t.GetComponent<Renderer>() != null)
                t.GetComponent<Renderer>().materials[0].SetFloat("_stepY", value * (completeEnd.position - completeStart.position).y);
        }
        //达到一百
        if (value >= 1)
        {
            complete = true;
        }
        return value;
    }
    Ray ray = new Ray();
    bool canSeeEnemy(Transform enemy)
    {
        if (_Bullet.GetComponent<Bullet>().Type == Bullet.BulletType.Persistent)
        {
            ray.origin = FirePoints[0].transform.position;
            ray.direction = enemy.position - FirePoints[0].transform.position;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 300f))
            {
                var v = hit.transform.GetComponent<Enemy>();
                if (v == null)
                {
                    return false;
                }
            }
        }
        return true;
    }
    /// <summary>
    /// 瞄准目标
    /// </summary>
    public void Aim()
    {
        //上一个敌人距离
        float goalDis = 0;
        //上一个敌人目标
        if (fireGoal != null)
            goalDis = (fireGoal.transform.position - transform.position).magnitude;
        //上一个敌人距离超出活力范围则取消目标
        if (goalDis > FireField * 10 * Lv) fireGoal = null;
        //遍历所有的敌人
        for (int i = 0; i < EnemyPool.childCount; i++)
        {
            Transform t = EnemyPool.GetChild(i).transform;
            //如果敌人没有激活跳过
            if (!t.gameObject.activeInHierarchy) continue;
            //敌我之间的距离
            Vector3 r = t.position - transform.position;
            //距离在火力范围之内
            if (r.magnitude < FireField * 10 * Lv)
            {
                //攻击
                isAttack = true;
                //射击视线是否无遮挡
                if (canSeeEnemy(t))
                    //如果当前目标为空，或者上一个目标的敌我距离超过火力范围，或者该敌人距离终点比当前目标更近
                    //就把目标替换成当前敌人
                    if (fireGoal == null
                        || goalDis > FireField * 10 * Lv
                        || t.GetComponent<Enemy>().Path.Count - t.GetComponent<Enemy>().currentPathWay <
                            fireGoal.GetComponent<Enemy>().Path.Count - fireGoal.GetComponent<Enemy>().currentPathWay)
                        fireGoal = t.transform;
                //如果目标仍然为空则跳过
                if (fireGoal == null) continue;
                //如果目标死亡则取消目标
                if (fireGoal.GetComponent<Enemy>().hp < 0) fireGoal = null;
                //如果射击视线被遮挡则取消目标
                else if (!canSeeEnemy(fireGoal.transform)) fireGoal = null;
            }
        }
        //如果目标为空停止攻击
        if (fireGoal == null)
        {
            isAttack = false;
            return;
        }
        else Head.LookAt(fireGoal);
    }

    public IEnumerator Attack()
    {
        //子弹
        GameObject obj = null;
        while (true)
        {
            //攻击冷却
            yield return new WaitForSeconds(1 / FireRate);
            BeforeAttack();
            if (isAttack)
            {
                //所有火力点都开火
                for (int j = 0; j < FirePoints.Length; j++)
                {
                    //优先从缓存池中取子弹 持续伤害的子弹始终为一个不需要重复取除非子弹为空
                    if (_Bullet.GetComponent<Bullet>().Type != Bullet.BulletType.Persistent || obj == null)
                        obj = FindBulletInHierarchy(_Bullet.GetComponent<Bullet>().Type);
                    //如果没有多余的子弹就创建
                    if (obj == null)
                    {
                        obj = GameObject.Instantiate(_Bullet);
                        obj.transform.position = FirePoints[j].position;
                        //放入缓存池
                        obj.transform.SetParent(BulletPool, true);
                    }
                    //拿到了就激活
                    else
                    {
                        obj.transform.position = FirePoints[j].position;
                        obj.SetActive(true);
                    }
                    //给子弹赋予伤害
                    obj.GetComponent<Bullet>().ATK = ATK;
                    //持续伤害子弹直接给目标造成伤害
                    if (obj.GetComponent<Bullet>().Type == Bullet.BulletType.Persistent)
                    {
                        //bug:持续伤害子弹不能lookAt敌人 否则会打不中
                        //GetChild(0)设定为闪电的碰撞体
                        obj.transform.GetChild(0).transform.position = fireGoal.transform.position;
                    }
                    //实体子弹赋予初速度
                    else if (obj.GetComponent<Bullet>().Type == Bullet.BulletType.Entity)
                    {
                        obj.GetComponent<Rigidbody>().velocity
                            = (FirePoints[j].position - FirePoints[j].parent.transform.position) * BulletVelocity * 10;
                    }
                    //子弹的发射位置
                    obj.GetComponent<Bullet>().startPos = FirePoints[j].position;
                    //子弹的目标位置
                    obj.GetComponent<Bullet>().endPos = fireGoal.transform.position;
                }
                AfterAttack();
            }
            //射击结束子弹失活
            else if (obj != null)
            {
                obj.SetActive(false);
                obj = null;
            }
        }
    }
    protected GameObject FindBulletInHierarchy(Bullet.BulletType type)
    {

        for (int j = 0; j < BulletPool.childCount; j++)
        {

            if (BulletPool.GetChild(j).GetComponent<Bullet>().Type == type)
            {
                if (BulletPool.GetChild(j).gameObject.activeInHierarchy == false)
                    return BulletPool.GetChild(j).gameObject;

            }
        }
        return null;
    }
    /// <summary>
    /// 攻击之后调用
    /// </summary>
    public abstract void AfterAttack();
    /// <summary>
    /// 攻击之前调用
    /// </summary>
    public abstract void BeforeAttack();
}
