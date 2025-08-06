using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//���̺� ����
[System.Serializable]
public class SpawnWave
{
    public string waveName;
    public float startTime;
    public float endTime;
    public float spawnInterval = 3.0f;
    [Header("���� ������")]
    public GameObject monsterPrefab;
    [Tooltip("���� ���̺� ���")]
    public bool isBoss;
    [HideInInspector]
    public float nextSpawnTime;
    [HideInInspector]
    public bool hasSpawned;     //���� ���� ����
}

public class EnemySpawner : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField]
    private float spawnRadius = 3f;       //���� �ݰ�

    [Header("�ð��뺰 ���� �ܰ�")]
    [SerializeField]
    private List<SpawnWave> spawnWaves; // �� ���̺��� ������ ��� �迭

    private TimeManager _timeManager;

    //�ʱ�ȭ
    private void Awake()
    {
        _timeManager = FindFirstObjectByType<TimeManager>();
    }

    private void Update()
    {
        float currentTime = _timeManager.ElapsedTime;

        foreach (var wave in spawnWaves)
        {
            if (wave.monsterPrefab == null || currentTime < wave.startTime ||
                currentTime > wave.endTime)
                continue;

            //���� ���̺� ����
            if (wave.isBoss)
            {
                if (!wave.hasSpawned)
                {
                    GameObject bossInstance = SpawnMonster(wave.monsterPrefab);
                    wave.hasSpawned = true;
                }
            }
            //�Ϲ� ���̺� ����
            else
            {
                if (currentTime >= wave.nextSpawnTime)
                {
                    SpawnMonster(wave.monsterPrefab);
                    wave.nextSpawnTime = currentTime + wave.spawnInterval;
                }
            }
        }
    }

    //-------------------- Monster Spawn --------------------
    GameObject SpawnMonster(GameObject monsterPrefab)
    {
        // ������ ������Ʈ�� ��ġ�� �������� ������ ������ ���� (X, Z ���)
        Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);
        return Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
    }
}