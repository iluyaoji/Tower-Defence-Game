using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    static LevelManager _Instance; //单例实现
    public static LevelManager Instance { get { return _Instance; } } //单例获取接口
    [Header("初始金币")]
    public float Level_Money;
    [Header("对象池")]
    public List<GameObject> Towers;
    public Transform EnemyPool;
    public Transform BulletPool;
    public SpawnEnemy[] SpawnEnemyScript;
    public Canvas _Canvas;
    public Transform Ground;
    [Header("血条和弹幕UI")]
    public GameObject UIBlood_red;
    public GameObject UIBlood_blue;
    public GameObject UIText;
    [Header("弹幕池")]
    public Transform UIBloodRoot;
    public Transform UITextRoot;
    [System.NonSerialized]
    public bool isPlay;
    [System.NonSerialized]
    public int RemainEnemy;
    [Header("计时器")]
    public Transform TimeText;

    void Awake()
    {
        if(_Instance == null)
        {
            _Instance = this;
            //重新加载场景只会一直累加，会多次执行SceneLoaded方法
            SceneManager.sceneLoaded += SceneLoaded;
        }
    }
    /// <summary>
    ///  在Awake之后调用,注意重新加载场景属性值是不会重新按照定义重新赋值的
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //重新加载场景，协程不会停，需要手动停止，不然可能会报MissingReferenceException
        //StopAllCoroutines();
        isPlay = false;
        //if (arg0.name == "StartScene" || arg0.name == "LevelView") return;
        //GameObject.Find("name")找不到物体的原因在于这个方法只能找到根目录下的，已显示的物体

        Stime = 0;
        Mtime = 0;
        //GamePlay();
        SceneManager.sceneLoaded -= SceneLoaded;
    }
    void FixedUpdate()
    {
        if (_Canvas == null) return;
        if(RemainEnemy <= 0)
        {
            _Canvas.GetComponent<UIClickScript>().SetingPlane.GetChild(0).GetComponent<Text>().text = "COMPLETE";
            if (isPlay) _Canvas.GetComponent<UIClickScript>().ClickButton_Pause();
        }
        if(isPlay)
            Timer();
    }

    public static List<Transform> FindAllChildren(Transform parent)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            children.Add(parent.GetChild(i));
            if (parent.GetChild(i).childCount > 0)
            {
                foreach(Transform child in FindAllChildren(parent.GetChild(i)))
                    children.Add(child);
            }
        }
        return children;
    }
    public void stopCoroutne()
    {
        StopAllCoroutines();
    }
    public void GamePlay()
    {
        if (isPlay) return;
        //5秒后开始游戏
        StartCoroutine(GamePlay(5f));
    }

    IEnumerator GamePlay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isPlay = true;
        for (int i = 0; i < SpawnEnemyScript.Length; i++)
            StartCoroutine(SpawnEnemyScript[i].Spawn());
    }

    float Stime;
    int Mtime;
    public void Timer()
    {
        Stime += Time.deltaTime;
        if ((int)Stime >= 60)
        {
            Stime = 0;
            Mtime += 1;
        }
        TimeText.GetComponent<Text>().text = Mtime + ":" + (int)Stime;
    }
}