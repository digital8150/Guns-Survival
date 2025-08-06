using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 일단 몬스터가 웨이브 별로 순서대로 나오는 것만 구현
/// 추후에 몬스터 체력이 점점 늘어서 스폰된다던지는 나중에 생각
/// </summary>

//웨이브 정보
[System.Serializable]
public class SpawnWave
{
    public string waveName;         //웨이브 명
    [Tooltip("스폰을 시작할 게임 시간(초)")]
    public float startTime;
    [Tooltip("스폰을 종료할 게임 시간(초)")]
    public float endTime;

    [Header("몬스터 설정")]
    public GameObject monsterPrefab;    //스폰할 몬스터

    [Tooltip("스폰 간격(초). 보스일 경우 무시")]
    public float spawnInterval = 3.0f;

    [Tooltip("체크 시, 단 한번 스폰되는 보스 몬스터로 취급")]
    public bool isBoss;

    [HideInInspector]
    public float nextSpawnTime;
    [HideInInspector]
    public bool hasSpawned;         //보스 스폰 여부
}

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField]
    private float spawnRadius = 3f;       //스폰 반경

    [Header("시간대별 스폰 단계")]
    [SerializeField]
    private List<SpawnWave> spawnWaves; // 각 웨이브의 정보를 담는 배열

    public static event Action<GameObject> OnBossSpawned;       //보스 스폰 이벤트

    private TimeManager _timeManager;
    private int currentWaveIndex = 0; // 현재 진행 중인 웨이브 인덱스
    private bool isSpawning = false; // 현재 몬스터 스폰 중인지 여부

    //초기화
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
            //몬스터 프리팹이 없거나, 현재 시간이 스폰 범위 밖이면 제외
            if (wave.monsterPrefab == null || currentTime < wave.startTime || currentTime > wave.endTime)
                continue;

            //보스 스폰 로직
            if(wave.isBoss)
            {
                if(!wave.hasSpawned)
                {
                    GameObject bossInstance = SpawnMonster(wave.monsterPrefab);
                    OnBossSpawned?.Invoke(bossInstance);
                    wave.hasSpawned = true;
                }     
            }
            //일반 스폰 로직
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

    //------------------------------------- 몬스터 스폰 ------------------------------------------
    GameObject SpawnMonster(GameObject monsterPrefab)
    {
        // 스포너 오브젝트의 위치를 기준으로 랜덤한 오프셋 생성 (X, Z 평면)
        Vector2 randomCirclePoint = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

        return Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
    }
}