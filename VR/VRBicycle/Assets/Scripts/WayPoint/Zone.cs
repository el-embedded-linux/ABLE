using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public GameObject EnemyPrefab;
    List<Enemy> EnemyList = new List<Enemy>();
    List<Transform> Waypoint = new List<Transform>();
    Transform enemys;

    float elapsedTime;
    public float NextWaveTime = 60;
    public int WaveEnemyMakeCount = 5;
    void Awake()
    {
        elapsedTime = 0;
        var childs = transform.Find("WayList").GetComponentsInChildren<Transform>();
        foreach (var child in childs)
        {
            if (!child.name.Contains("Point")) continue;
            Waypoint.Add(child);
        }

        CreateEnemy(20);
        StartCoroutine(StartWave());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (NextWaveTime < elapsedTime)
        {
            elapsedTime = 0;
            StartCoroutine(StartWave());
        }
    }

    void CreateEnemy(int count)
    {
        if (enemys == null)
        {
            var obj = new GameObject();
            obj.name = "Enemys";
            obj.transform.parent = transform;
            enemys = obj.transform;
        }

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(EnemyPrefab);
            if (obj == null) return;

            obj.name = "Enemy" + i;
            obj.transform.parent = enemys;
            obj.SetActive(false);
            EnemyList.Add(obj.GetComponent<Enemy>());
        }
    }

    IEnumerator StartWave()
    {
        float elapsedTime = 0;
        int index = 0;
        while (WaveEnemyMakeCount > index)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 0.8f)
            {
                elapsedTime = 0;
                ++index;
                MakeEnemy();
            }
            yield return null;
        }
    }

    void MakeEnemy()
    {
        var obj = FindNoUseEnemy();
        if (obj == null)
            return;

        obj.Init(Waypoint);
        return;
    }

    Enemy FindNoUseEnemy()
    {
        for (int i = 0; i < EnemyList.Count; i++)
        {
            if (EnemyList[i].gameObject.activeSelf)
                continue;
            return EnemyList[i];
        }
        return null;
    }
}



