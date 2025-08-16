using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//웨이브 정보
[System.Serializable]
public class SpawnWave
{
    public string waveName;
    public float startTime;
    public float endTime;
    public float spawnInterval = 3.0f;
    [Header("몬스터 프리팹")]
    public GameObject monsterPrefab;
    [Tooltip("보스 웨이브 취급")]
    public bool isBoss;
    [Tooltip("파이널 보스 웨이브 취급")]
    public bool isFinalBoss;

    [HideInInspector]
    public float nextSpawnTime;
    [HideInInspector]
    public bool hasSpawned;     //보스 스폰 여부
}

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField]
    private float spawnRadius = 3f;       //스폰 반경

    [Header("시간대별 스폰 단계")]
    [SerializeField]
    private List<SpawnWave> spawnWaves; // 각 웨이브의 정보를 담는 배열

    [Header("컴포넌트 참조")]
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private UIManager _UIManager;

    private TimeManager _timeManager;

    //초기화
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

            //보스 웨이브 로직
            if (wave.isBoss || wave.isFinalBoss)
            {
                if (!wave.hasSpawned)
                {
                    GameObject bossInstance = SpawnMonster(wave.monsterPrefab);
                    _UIManager.RegisterBossObject(bossInstance);
                    wave.hasSpawned = true;
                }
            }
            //일반 웨이브 로직
            else
            {
                if (currentTime >= wave.nextSpawnTime)
                {
                    var clone = SpawnMonster(wave.monsterPrefab);
                    wave.nextSpawnTime = currentTime + wave.spawnInterval;
                }
            }
        }
    }

    //-------------------- Monster Spawn --------------------
    GameObject SpawnMonster(GameObject monsterPrefab)
    {
        // 스포너 오브젝트의 위치를 기준으로 랜덤한 오프셋 생성 (X, Z 평면)
        Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

        GameObject mobInstance = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        EnemyAI mobAI = mobInstance.GetComponent<EnemyAI>();
        if(mobAI != null)
        {
            mobAI.PlayerTransform = playerTransform;
        }
        return mobInstance;
    }
}