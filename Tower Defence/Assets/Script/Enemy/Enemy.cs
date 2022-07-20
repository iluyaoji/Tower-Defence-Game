using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public bool isDebug;
    public float price;
    public ENUME_EnemyType type;
    public float HP = 10;
    public float Speed = 1;
    public Transform mountPoint;

    [System.NonSerialized] public List<Node> Path;
    [System.NonSerialized] public float BurnAmount = 0;
    [System.NonSerialized] public float hp;
    [System.NonSerialized] public Renderer rd = null;
    [System.NonSerialized] public GameObject UIBlood;

    private Transform UITextRoot;
    private GameObject UIText;
    private UICharactor _UIBloodC;
    public int currentPathWay = 1;

    private LevelManager levelManager;
    public SpawnEnemy spawnEnemy;
    /*
    void Start()
    {
        hp = HP;
        rd = gameObject.GetComponentInChildren<Renderer>();
        gameManager = FindObjectOfType<GameManager>();
        UIText = gameManager.UIText;
        UIBlood = GameObject.Instantiate(gameManager.UIBlood_red);
        UITextRoot = gameManager.UITextRoot;
        _UIBloodC = UIBlood.GetComponent<UICharactor>();
        _UIBloodC.UIRoot = gameManager.UIBloodRoot;
        Path = spawnEnemy.GetComponent<PathFinder>().BuildPath();
        
        _UIBloodC.instanceParentPos = this.mountPoint.transform;
    }
    */
    public void Initial()
    {
        hp = HP;
        rd = gameObject.GetComponentInChildren<Renderer>();
        levelManager = LevelManager.Instance;
        UIText = levelManager.UIText;
        UIBlood = GameObject.Instantiate(levelManager.UIBlood_red);
        UITextRoot = levelManager.UITextRoot;
        _UIBloodC = UIBlood.GetComponent<UICharactor>();
        _UIBloodC.UIRoot = levelManager.UIBloodRoot;

        RePath();
        _UIBloodC.instanceParentPos = this.mountPoint.transform;
    }
    /// <summary>
    /// 重置路径
    /// </summary>
    public void RePath()
    {
        Path = spawnEnemy.gameObject.GetComponent<PathFinder>().BuildPath();
    }

    void Update()
    {
        if (isDebug) return;
        if (hp < 0)
        {
            //播放燃烧动画
            if (BurnAmount < 1)
            {
                BurnAmount += Time.deltaTime;
                rd.materials[0].SetFloat("_BurnAmount", BurnAmount);
            }
            else
            {
                GameEventSystem.Notify(Enum_GameEvent.KillEnemy,type);
                UIBlood.SetActive(false);
                currentPathWay = 1;
                levelManager.Level_Money += price;
                ShowSubtitle("+￥" + price.ToString()).GetComponent<Text>().color = Color.yellow;
                levelManager.RemainEnemy -= 1;
                gameObject.SetActive(false);
            }
            return;
        }
        UIBlood.GetComponent<UICharactor>().SetValue(hp / HP);
        if (currentPathWay < Path.Count)
        {
            Move();
        }
        else
        {
            levelManager._Canvas.GetComponent<UIClickScript>().SetingPlane.GetChild(0).GetComponent<Text>().text = "GAMEOVER";
            if(levelManager.isPlay) levelManager._Canvas.GetComponent<UIClickScript>().ClickButton_Pause();
        }
        //更新血条位置
        _UIBloodC.instanceParentPos = this.mountPoint.transform;
    }


    void Move()
    {
        Vector3 currentPath = 
            new Vector3((float)Path[currentPathWay].coordiates.x, 0, (float)Path[currentPathWay].coordiates.y);
        currentPath *= 10;

        currentPath.y += transform.position.y;
        transform.LookAt(currentPath);
        Vector3 direct = currentPath - transform.position;
        
        if (direct.magnitude > 0.1)
            transform.position += direct.normalized * Time.deltaTime * Speed;
        else currentPathWay++;
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (isDebug) return;
        if(other.transform.tag == "Fire")
        {
            float atk = other.transform.GetComponent<Bullet>().ATK;
            //other.gameObject.SetActive(false);
            hp -= atk;
            ShowSubtitle(atk.ToString()).GetComponent<Text>().color 
                = other.transform.GetComponent<Bullet>().DamageTextColor;
            
            //这样写的话 当该脚本Active为false时 协同也就关闭(可能是暂停)了
            //StartCoroutine(num.GetComponent<UICharactor>().Move());
            //UItext.GetComponent<UICharactor>().ActionMove();
        }

    }
    private GameObject FindUItextInHierarchy()
    {

        for (int j = 0; j < UITextRoot.childCount; j++)
        {
            
            if (UITextRoot.GetChild(j).gameObject.activeInHierarchy == false)
                return UITextRoot.GetChild(j).gameObject;
        }
        return null;
    }

    GameObject ShowSubtitle(string text)
    {
        GameObject UItext = FindUItextInHierarchy();
        if (UItext == null)
        {
            UItext = GameObject.Instantiate(UIText);
            UItext.GetComponent<UICharactor>().UIRoot = UITextRoot;
        }
        UItext.GetComponent<UICharactor>().instanceParentPos = transform;
        UItext.GetComponent<UICharactor>().ResetPos();
        UItext.GetComponent<Text>().text = text;
        UItext.SetActive(true);
        return UItext;
    }
}



