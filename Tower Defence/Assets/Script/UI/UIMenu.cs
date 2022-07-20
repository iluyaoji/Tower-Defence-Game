using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIMenu : MonoBehaviour
{
    public GameObject UI_LevelView;
    public GameObject UI_GuideView;
    public GameObject UI_TaskView;

    public void ClickSelect(int index)
    {
        GameManager.Instance.LoadingScene(index+1);
    }

    public void BallButtonOnClick(int UI_Index)
    {
        switch (UI_Index)
        {
            case 1:
                UI_LevelView.SetActive(true);
                UI_GuideView.SetActive(false);
                UI_TaskView.SetActive(false);
                break;
            case 2:
                UI_LevelView.SetActive(false);
                UI_GuideView.SetActive(true);
                UI_TaskView.SetActive(false);
                break;
            case 3:
                UI_LevelView.SetActive(false);
                UI_GuideView.SetActive(false);
                UI_TaskView.SetActive(true);
                break;
        }
    }
}
