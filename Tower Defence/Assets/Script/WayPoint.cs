using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public GameObject SelectItemUI;
    public bool isConstructable;

    [SerializeField] public Node node;
    private void Start()
    {
        SelectItemUI = FindObjectOfType<Canvas>().transform.Find("UIClickToSeclet").Find(SelectItemUI.name).gameObject;
    }
    private void Update()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            SelectItemUI.gameObject.SetActive(false);
        }
    }
    
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!SelectItemUI.activeInHierarchy)
            if (isConstructable)
            {

                Vector3 worldPos = this.transform.position;
                Vector3 screenPos = FindObjectOfType<Camera>().WorldToScreenPoint(worldPos);
                SelectItemUI.GetComponent<UICharactor>().ShowAt(screenPos);
                SelectItemUI.gameObject.SetActive(true);
                SelectItemUI.GetComponent<ClickScript>().trans = transform;
            }
        }

    }
    
}
