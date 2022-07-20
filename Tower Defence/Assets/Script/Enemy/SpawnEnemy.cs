using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [System.Serializable]
    public struct SequenceTable
    {
        public ENUME_EnemyType type;
        public float DelaySpawnTime;
    }
    private float spawnTimer;
    public List<GameObject> enemyPrefab;
    public List<SequenceTable> enemySequence;

    int EnemyCount = 0;
    //由于是在SceneLoaded中调用，所以在Start函数还没执行他就执行了
    public IEnumerator Spawn()
    {
        EnemyCount = 0;
        LevelManager.Instance.RemainEnemy += enemySequence.Count;
        //生成敌人之前先初始化路径
        GetComponent<PathFinder>().Initial();
        while (EnemyCount < enemySequence.Count)
        {
            spawnTimer = enemySequence[EnemyCount].DelaySpawnTime;
            yield return new WaitForSeconds(spawnTimer);
            EnableObjectInPool(EnemyCount++);
        }
    }

    private void EnableObjectInPool(int index)
    {
        //if (this == null) return;
        GameObject enemy = FindEnemyInHierarchy(enemySequence[index].type);
        
        if (enemy != null)
        {
            enemy.GetComponent<Enemy>().hp = enemy.GetComponent<Enemy>().HP;
            enemy.transform.position = transform.position;
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().UIBlood.SetActive(true);
            enemy.GetComponent<Enemy>().rd.materials[0].SetFloat("_BurnAmount", 0);
            enemy.GetComponent<Enemy>().BurnAmount = 0;
            enemy.GetComponent<Enemy>().spawnEnemy = this;
            enemy.GetComponent<Enemy>().RePath();
        }
        else
        {
            var perfeb = FindEnemyPrefeb(enemySequence[index].type);
            if (!perfeb)
            {
               
                return;
            }
            enemy = Instantiate(perfeb, transform.position, Quaternion.identity);
            enemy.GetComponent<Enemy>().spawnEnemy = this;
            enemy.GetComponent<Enemy>().Initial();
            enemy.transform.parent = LevelManager.Instance.EnemyPool;
            enemy.SetActive(true);

        }
        
    }

    private GameObject FindEnemyInHierarchy(ENUME_EnemyType type)
    {
        for (int j = 0; j < LevelManager.Instance.EnemyPool.transform.childCount; j++)
        {
            if (LevelManager.Instance.EnemyPool.transform.GetChild(j).GetComponent<Enemy>().type == type && LevelManager.Instance.EnemyPool.transform.GetChild(j).gameObject.activeInHierarchy == false)
            {
                return LevelManager.Instance.EnemyPool.transform.GetChild(j).gameObject;
            }
        }
        return null;
    }
    private GameObject FindEnemyPrefeb(ENUME_EnemyType type)
    {

        for (int j = 0; j < enemyPrefab.Count; j++)
        {
            if (enemyPrefab[j].GetComponent<Enemy>().type == type)
                return enemyPrefab[j];
        }
        return null;
    }

}
