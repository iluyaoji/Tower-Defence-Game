using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIClickScript : MonoBehaviour
{
    public Transform VFX_Select;
    [ColorUsageAttribute(true, true)]
    public Color ColorYes = Color.white;
    [ColorUsageAttribute(true, true)]
    public Color ColorNo = Color.white;

    public Transform MoneyText;
    public Transform GameButton;
    public Transform SetingPlane;
    private List<GameObject> Towers;
    private Transform o;
    private bool isSelect = false;
    void Start()
    {
        //这样一来即共享了材质又没有改变asset中的材质
        VFX_Select.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", ColorNo);
        foreach(Transform t in LevelManager.FindAllChildren(VFX_Select.GetChild(1)))
        {
            t.GetComponent<ParticleSystemRenderer>().material 
                = VFX_Select.GetChild(0).GetComponent<Renderer>().material;
        }
        Towers = LevelManager.Instance.Towers;
        
    }
    private void Update()
    {
        MoneyText.GetComponent<Text>().text ="￥" + LevelManager.Instance.Level_Money.ToString();
        if (isSelect)
        {
            o.GetComponent<ITower>().FireFieldView.gameObject.SetActive(true);
            Transform t;
            if (DragO(out t)&&Input.GetMouseButtonDown(0))
            {
                if (LevelManager.Instance.Level_Money - o.GetComponent<ITower>().price < 0)
                {
                    return;
                }
                isSelect = false;
                t.GetComponent<WayPoint>().isConstructable = false;
                o.GetComponent<ITower>().FireFieldView.gameObject.SetActive(false);
                o.GetComponent<ITower>().enabled = true;
                LevelManager.Instance.Level_Money -= o.GetComponent<ITower>().price;
                o = null;
            }
            if (Input.GetMouseButtonDown(1))
            {
                isSelect = false;
                o.gameObject.SetActive(false);
            }
        }
        MousePoint();
    }

    void MousePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 300f, 512))
        {
            var v = hit.transform.GetComponent<WayPoint>();
            if (v != null)
            {
                if(v.isConstructable)
                {
                    if(isSelect && LevelManager.Instance.Level_Money - o.GetComponent<ITower>().price < 0)
                        VFX_Select.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetColor("_Color", ColorNo);
                    else 
                        VFX_Select.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetColor("_Color", ColorYes);
                }
                else VFX_Select.GetChild(0).GetComponent<Renderer>().sharedMaterial.SetColor("_Color", ColorNo);
                
                VFX_Select.gameObject.SetActive(true);
                VFX_Select.transform.position = hit.transform.position;
            }
        }
        else
        {
            VFX_Select.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 鼠标不断发出射线碰撞检测
    /// </summary>
    /// <param name="hit_obj"></param>
    /// <returns></returns>
    private bool DragO(out Transform hit_obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 300f, 512))
        {
            hit_obj = hit.transform;
            var v = hit.transform.GetComponent<WayPoint>();
            if (v != null)
            {
                if (o != null)
                    o.transform.position = hit.transform.position;
                if(!v.isConstructable) return false;
            }
        }
        else
        {
            hit_obj = null;
            return false;
        }
        return true;
    }
    
    public void ClickButton_weapon(int type)
    {
        if (isSelect) return;
        o = Instantiate(Towers[type], new Vector3(-20, 0, -20), Quaternion.identity).transform;
        o.GetComponent<ITower>().enabled = false;
        isSelect = true;
    }

    public void ClickButton_Pause()
    {
        if (LevelManager.Instance.isPlay)
        {
            Time.timeScale = 0;
            LevelManager.Instance.isPlay = false;
            GameButton.gameObject.SetActive(false);
            SetingPlane.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            LevelManager.Instance.isPlay = true;
            GameButton.gameObject.SetActive(true);
            SetingPlane.gameObject.SetActive(false);
        }
    }

    public void ClickButton_ReStart()
    {
        Time.timeScale = 1;
        LevelManager.Instance.stopCoroutne();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ClickButton_Exit()
    {
        Time.timeScale = 1;
        GameManager.Instance.LoadingScene(0);
        //Application.Quit();
    }
}
