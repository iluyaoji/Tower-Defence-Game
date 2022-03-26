using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int type;
    public float HP = 10;
    public float Speed = 1;
    public GameObject UI;
    public Transform mountPoint;
    public Renderer rd = null;

    public List<Node> Path;

    private UICharactor uiBlood;
    private int currentPathWay = 1;
    [System.NonSerialized]public float BurnAmount = 0;
    [System.NonSerialized] public float hp;

    void Start()
    {
        hp = HP;
        UI = GameObject.Instantiate(UI);

        uiBlood = UI.GetComponent<UICharactor>();
    }

    void FixedUpdate()
    {
        if (hp < 0)
        {
            BurnAmount += Time.deltaTime;
            
            if (BurnAmount < 1)
            {
                rd.materials[0].SetFloat("_BurnAmount", BurnAmount);
            }

        }
    }

    void Update()
    {
        if (hp < 0)
        {
            if (BurnAmount < 1) return;
            currentPathWay = 1;
            gameObject.SetActive(false);
            UI.SetActive(false);
        }
        UI.GetComponent<UICharactor>().SetValue(hp / HP);
        if (currentPathWay < Path.Count)
        {
            Move();
        }
    }
    private void LateUpdate()
    {
        Vector3 worldPos = this.mountPoint.transform.position;
        Vector3 screenPos = FindObjectOfType<Camera>().WorldToScreenPoint(worldPos);
        uiBlood.ShowAt(screenPos);
    }
    void Move()
    {
        Vector3 currentPath = 
            new Vector3((float)Path[currentPathWay].coordiates.x, 0, (float)Path[currentPathWay].coordiates.y);
        currentPath *= UnityEditor.EditorSnapSettings.move.x;
        Vector3 direct = currentPath - transform.position;
        
        direct.y += transform.position.y;
        if (direct.magnitude > 0.1)
            transform.position += direct.normalized * Time.deltaTime * Speed;
        else currentPathWay++;
        
    }
    private void OnParticleCollision(GameObject other)
    {
        if(other.tag == "Fire")
        {
            hp -= other.GetComponentInParent<Tower>().ATK;
        }

    }
}



