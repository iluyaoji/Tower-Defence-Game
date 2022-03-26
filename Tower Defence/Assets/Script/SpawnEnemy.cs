using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public List<GameObject> enemyPrefab;
    public List<int> enemySequence;
    public float spawnTimer = 1f;

    
    void Start()
    {
        
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int i = 0;
        while (i<enemySequence.Count)
        {
            EnableObjectInPool(i++);
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    private void EnableObjectInPool(int index)
    {
        GameObject enemy = FindEnemyInHierarchy(enemySequence[index]);
        
        if (enemy != null)
        {
            enemy.GetComponent<Enemy>().hp = enemy.GetComponent<Enemy>().HP;
            enemy.transform.position = transform.position;
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().UI.SetActive(true);
            enemy.GetComponent<Enemy>().rd.materials[0].SetFloat("_BurnAmount", 0);
            enemy.GetComponent<Enemy>().BurnAmount = 0;
        }
        else
        {
            enemy = Instantiate(FindEnemyPrefeb(enemySequence[index]), transform.position, Quaternion.identity);
            enemy.GetComponent<Enemy>().Path = GetComponent<PathFinder>().BuildPath();
            enemy.transform.parent = transform;
            enemy.SetActive(true);

        }
        
    }

    private GameObject FindEnemyInHierarchy(int type)
    {
        for (int j = 0; j < transform.childCount; j++)
        {
            if (transform.GetChild(j).GetComponent<Enemy>().type == type && transform.GetChild(j).gameObject.activeInHierarchy == false)
                return transform.GetChild(j).gameObject;
        }
        return null;
    }
    private GameObject FindEnemyPrefeb(int type)
    {
        for (int j = 0; j < enemyPrefab.Count; j++)
        {
            if (enemyPrefab[j].GetComponent<Enemy>().type == type)
                return enemyPrefab[j];
        }
        return null;
    }
}
