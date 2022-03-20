using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScript : MonoBehaviour
{
    public List<Tower> Towers;
    [System.NonSerialized]
    public Transform trans;

    /*
    private Transform o;
    private bool isSelect = false;
    private void Update()
    {
        if (isSelect)
        {
            DragO();
        }
        if (isSelect&&Input.GetMouseButtonDown(0))
        {
            isSelect = false;
            o.GetComponent<Tower>().enabled = true;
        }
    }

    
    private void DragO()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            o.transform.position = hit.transform.position;
        }
    }
    */
    public void ClickButton_1()
    {
        Instantiate(Towers[0], trans.position, Quaternion.identity);
        gameObject.SetActive(false);
        trans.GetComponent<WayPoint>().isConstructable = false;
    }

    public void ClickButton_2()
    {
        Instantiate(Towers[1], trans.position, Quaternion.identity);
        gameObject.SetActive(false);
        trans.GetComponent<WayPoint>().isConstructable = false;
    }

    /*
    public void ClickButton_3()
    {
        o = Instantiate(Towers[0],new Vector3(-20,0,-20), Quaternion.identity).transform;
        o.GetComponent<Tower>().enabled = false;
        //gameObject.SetActive(false);
        isSelect = true;
    }
    */
}
