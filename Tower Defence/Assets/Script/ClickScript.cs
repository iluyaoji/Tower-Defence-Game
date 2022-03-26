using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScript : MonoBehaviour
{
    public List<Tower> Towers;
    public Material TowerMaterial;
    [System.NonSerialized]

    
    private Transform o;
    private Dictionary<int, Material> TowerMaterilList = new Dictionary<int, Material>();
    private bool isSelect = false;
    private void Update()
    {
        if (isSelect)
        {
            Transform t;
            if (DragO(out t)&&Input.GetMouseButtonDown(0))
            {
                isSelect = false;
                t.GetComponent<WayPoint>().isConstructable = false;
                for (int i = 0; i < o.childCount; i++)
                {
                    o.GetChild(i).GetComponent<Renderer>().material = TowerMaterilList[i];
                    TowerMaterilList.Remove(i);
                }
                o.GetComponent<Tower>().enabled = true;
            }
            if (Input.GetMouseButtonDown(1))
            {
                isSelect = false;
                o.gameObject.SetActive(false);
                TowerMaterilList.Clear();
            }
        }
    }

    
    private bool DragO(out Transform hit_obj)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f,1))
        {
            hit_obj = hit.transform;
            var v = hit.transform.GetComponent<WayPoint>();
            if (v!=null&&v.isConstructable)
                o.transform.position = hit.transform.position;
            else return false;
        }
        else
        {
            hit_obj = null;
            return false;
        }
        return true;
    }
    
    public void ClickButton_1()
    {
        if (isSelect) return;
        o = Instantiate(Towers[0],new Vector3(-20,0,-20), Quaternion.identity).transform;
        o.GetComponent<Tower>().enabled = false;
        for(int i = 0; i < o.childCount; i++)
        {
            TowerMaterilList.Add(i, o.GetChild(i).GetComponent<MeshRenderer>().materials[0]);

            o.GetChild(i).GetComponent<Renderer>().material = TowerMaterial;
            /*
             * 这个表示找到对应的材质但是不能替换材质球
             * o.GetChild(i).GetComponent<MeshRenderer>().materials[0] = TowerMaterial;
             */
        }
        //gameObject.SetActive(false);
        isSelect = true;
    }

    public void ClickButton_2()
    {
        if (isSelect) return;
        o = Instantiate(Towers[1], new Vector3(-20, 0, -20), Quaternion.identity).transform;
        o.GetComponent<Tower>().enabled = false;
        for (int i = 0; i < o.childCount; i++)
        {
            TowerMaterilList.Add(i, o.GetChild(i).GetComponent<MeshRenderer>().materials[0]);

            o.GetChild(i).GetComponent<Renderer>().material = TowerMaterial;
        }
        //gameObject.SetActive(false);
        isSelect = true;
    }
}
