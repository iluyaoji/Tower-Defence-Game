using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int Lv = 1;
    public float ATK = 1;
    //[SerializeField] GameObject ClickUI;
    public Transform Head;
    public float FireField = 1;
    public float buildDelay = 1;

    public Transform mountPoint;
    [SerializeField] GameObject UIDegreeOfCompletion;
    public ParticleSystem[] particleSystems;
    public float emissionRate;

    private Enemy[] Target;
    bool complete = false;
    float degreeOfCompletion = 0;
    
    private void Start()
    {
        UIDegreeOfCompletion = GameObject.Instantiate(UIDegreeOfCompletion);
        
    }
    void FixedUpdate()
    {
        if (!complete)
            UIDegreeOfCompletion.GetComponent<UICharactor>().SetValue(Building());
    }

    void Update()
    {
        if (complete)
            Aim();
    }
    public float Building()
    {
        degreeOfCompletion += Time.deltaTime;
        float value = degreeOfCompletion / buildDelay;
        Vector3 worldPos = this.mountPoint.transform.position;
        Vector3 screenPos = FindObjectOfType<Camera>().WorldToScreenPoint(worldPos);
        UIDegreeOfCompletion.GetComponent<UICharactor>().ShowAt(screenPos);
        if (value >= 1)
        {
            complete = true;
            //TODO: Optimized treatment recovery
            Destroy(UIDegreeOfCompletion);
        }
        return value;
    }

    void Aim()
    {
        Target = FindObjectsOfType<Enemy>();
        if (Target.Length <= 0)
        {
            foreach (ParticleSystem PSystem in particleSystems)
                PSystem.Stop();
        }
        float min = FireField*10*Lv;
        Transform firePoint = Head.transform;
        for (int i = 0; i < Target.Length; i++)
        {
            
            Transform t = Target[i].transform;
            Vector3 r = t.position - transform.position;
            
            if (r.magnitude < min)
            {
                min = r.magnitude;
                firePoint = t.transform;
                foreach (ParticleSystem PSystem in particleSystems)
                {

                    if (!PSystem.isPlaying)
                    {
                        PSystem.Play();
                    }
                    var p = PSystem.emission;
                    p.rateOverTime = emissionRate;
                }
            }
            
        }
        Head.LookAt(firePoint);
        
    }

}
