using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int Lv = 1;
    public float ATK = 1;
    [SerializeField] GameObject ClickUI;
    public Transform Head;
    public float FireField = 1;
    public ParticleSystem[] particleSystems;
    public float emissionRate;

    private Enemy[] Target;

    void Update()
    {
        Aim();
        
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

    private void OnMouseOver()
    {
        
    }
}
