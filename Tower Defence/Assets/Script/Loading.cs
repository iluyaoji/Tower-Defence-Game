using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    private AsyncOperation operation;//定义一个异步加载变量
    //进度条
    private Slider loadingBar;
    private Text loadingText;

    private float targetValue = 0;

    public float lerpSpeed = 2;
    void Start()
    {
        loadingBar = transform.Find("LoadingBar").GetComponent<Slider>();
        loadingText = transform.Find("LoadingBar/LoadingText").GetComponent<Text>();

        StartCoroutine("LoadSceneAsync");
    }

    // Update is called once per frame
    void Update()
    {
        targetValue = operation.progress;
        if (targetValue >= 0.9f)//当加载的值大于等于0.9的时候
        {
            targetValue = 1;//让目标值等于1
        }
        //设置进度条的值
        loadingBar.value = Mathf.Lerp(loadingBar.value, targetValue, lerpSpeed * Time.deltaTime);
        //设置文本的显示
        loadingText.text = (loadingBar.value * 100).ToString("F0") + "%";
        if ((targetValue - loadingBar.value) < 0.01f && targetValue == 1)//表示快接近目标值
        {
            loadingBar.value = 1;//设置进度条为1
            loadingText.text = (int)(loadingBar.value * 100) + "%";
            operation.allowSceneActivation = true;//显示加载完成的场景
        }
    }

    //异步加载方法
    IEnumerator LoadSceneAsync()
    {
        string targetName = GameManager.Instance.SceneList[GameManager.Instance.currentSceneIndex].SceneName;
        //Debug.Log(targetName);
        operation = SceneManager.LoadSceneAsync(targetName);
        //当场景异步加载完成时是否马上显示。禁用。需要等待Loading走到100%
        operation.allowSceneActivation = false;

        yield return operation;
    }
}
