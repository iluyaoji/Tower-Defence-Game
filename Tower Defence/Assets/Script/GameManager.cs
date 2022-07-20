using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(TaskManager))]
[RequireComponent(typeof(GameEventSystem))]
public class GameManager : MonoBehaviour
{
    static GameManager _Instance; //单例实现
    static public GameManager Instance { get { return _Instance; } } //单例获取接口
    //场景列表 第0个放菜单场景 关卡场景从1开始
    public List<GameScene> SceneList;
    //每个关卡的管理器
    public LevelManager LevelManager;
    public TaskManager TaskManager;
    public GameEventSystem GameEventSystem;

    public int currentSceneIndex;
    private void Awake()
    {
        if (_Instance == null)
            _Instance = this;
        TaskManager = GetComponent<TaskManager>();
        GameEventSystem = GetComponent<GameEventSystem>();
        SceneManager.sceneLoaded += SceneLoaded;
        DontDestroyOnLoad(gameObject);
    }
    public void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        TaskManager.Initial();
        if (FindObjectOfType<LevelManager>() == null) return;
        LevelManager = FindObjectOfType<LevelManager>();
        LevelManager.Level_Money = SceneList[currentSceneIndex].SceneMoney;
        LevelManager.Towers = SceneList[currentSceneIndex].Towers;
        LevelManager.GamePlay();
    }
    /// <summary>
    /// 加载场景，自定义的场景列表，与编辑器无关
    /// </summary>
    /// <param name="index"></param>
    public void LoadingScene(int index)
    {
        //SceneManager.LoadScene(SceneList[index].SceneName);
        currentSceneIndex = index;
        SceneManager.LoadScene("Loading");
    }
}
[System.Serializable]
public class GameScene
{
    public string SceneName;

    public int SceneMoney;

    public List<GameObject> Towers;

}
