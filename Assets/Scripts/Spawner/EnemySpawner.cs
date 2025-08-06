using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// �ϴ� ���Ͱ� ���̺� ���� ������� ������ �͸� ����
/// ���Ŀ� ���� ü���� ���� �þ �����ȴٴ����� ���߿� ����
/// </summary>

//���̺� ����
[System.Serializable]
public class SpawnWave
{
    public string waveName;         //���̺� ��
    [Tooltip("������ ������ ���� �ð�(��)")]
    public float startTime;
    [Tooltip("������ ������ ���� �ð�(��)")]
    public float endTime;

    [Header("���� ����")]
    public GameObject monsterPrefab;    //������ ����

    [Tooltip("���� ����(��). ������ ��� ����")]
    public float spawnInterval = 3.0f;

    [Tooltip("üũ ��, �� �ѹ� �����Ǵ� ���� ���ͷ� ���")]
    public bool isBoss;

    [HideInInspector]
    public float nextSpawnTime;
    [HideInInspector]
    public bool hasSpawned;         //���� ���� ����
}

public class EnemySpawner : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField]
    private float spawnRadius = 3f;       //���� �ݰ�

    [Header("�ð��뺰 ���� �ܰ�")]
    [SerializeField]
    private List<SpawnWave> spawnWaves; // �� ���̺��� ������ ��� �迭

    public static event Action<GameObject> OnBossSpawned;       //���� ���� �̺�Ʈ

    private TimeManager _timeManager;
    private int currentWaveIndex = 0; // ���� ���� ���� ���̺� �ε���
    private bool isSpawning = false; // ���� ���� ���� ������ ����

    //�ʱ�ȭ
    private void Awake()
    {
        _timeManager = FindFirstObjectByType<TimeManager>();
    }

    private void Update()
    {
        if (_timeManager == null)
            return;

        float currentTime = _timeManager.ElapsedTime;

        foreach (var wave in spawnWaves)
        {
            //���� �������� ���ų�, ���� �ð��� ���� ���� ���̸� ����
            if (wave.monsterPrefab == null || currentTime < wave.startTime || currentTime > wave.endTime)
                continue;

            //���� ���� ����
            if(wave.isBoss)
            {
                if(!wave.hasSpawned)
                {
                    GameObject bossInstance = SpawnMonster(wave.monsterPrefab);
                    OnBossSpawned?.Invoke(bossInstance);
                    wave.hasSpawned = true;
                }     
            }
            //�Ϲ� ���� ����
            else
            {
                if(currentTime >= wave.nextSpawnTime)
                {
                    SpawnMonster(wave.monsterPrefab);
                    wave.nextSpawnTime = currentTime + wave.spawnInterval;
                }
            }
        }
    }

    //------------------------------------- ���� ���� ------------------------------------------
    GameObject SpawnMonster(GameObject monsterPrefab)
    {
        // ������ ������Ʈ�� ��ġ�� �������� ������ ������ ���� (X, Z ���)
        Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

        return Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
    }
}