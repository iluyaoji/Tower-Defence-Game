using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletType
    {
        Entity,
        Persistent,
        Cannon
    };
    public float ATK;
    public Color DamageTextColor = Color.white;
    public BulletType Type;

    public Vector3 startPos;
    public Vector3 endPos;

    float lifeTime;
    void Update()
    {
        if (Type == BulletType.Persistent)
        {
            
            foreach(Thunder t in GetComponentsInChildren<Thunder>())
            {
                t.startPos = startPos;
                t.endPos = endPos;
                t.LerpPos();
            }
        }
        else if (Type == BulletType.Cannon)
        {
            if (lifeTime > 3) return;
            lifeTime += 5*Time.deltaTime;
            transform.position = Cannon(lifeTime);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.Equals("Fire")) return;
        //StopAllCoroutines();
        if (Type == BulletType.Entity)
        {
            //StopAllCoroutines();
            this.transform.GetComponent<Collider>().enabled = false;
            this.transform.GetChild(1).transform.GetComponent<ParticleSystem>().Play(); 

            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        else if(Type == BulletType.Persistent)
        {
            transform.GetChild(0).transform.position = startPos;
        }
        else
        {
            lifeTime = 0;
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (Type == BulletType.Entity)
            StartCoroutine(DelaySetActive(2.0f));
    }

    public IEnumerator DelaySetActive(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        transform.GetComponent<Collider>().enabled = true;
        this.transform.GetChild(0).gameObject.SetActive(true);
    }

    public Vector3 Cannon(float x)
    {
        float dis = (endPos - startPos).magnitude;
        float p = dis/2.0f;
        float y = Mathf.Pow(x*dis-p, 2) / (-2 * p) + p/2;
        Vector3 vector =startPos + (endPos - startPos)*x + transform.parent.TransformPoint(new Vector3(0, y, 0));
        return vector;
    }
}
