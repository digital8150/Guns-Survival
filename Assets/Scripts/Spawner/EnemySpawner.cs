using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Pool;
using System.Threading.Tasks;

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

    //----------- 오브젝트 풀링 ----------
    //여러 스포너 간에 공유하는 오브젝트 풀 딕셔너리 (프리팹별로 오브젝트 풀 관리)
    private static Dictionary<GameObject, ObjectPool<GameObject>> enemyPools = new Dictionary<GameObject, ObjectPool<GameObject>>();

    public static async void ReturnEnemyToPool(GameObject prefab, GameObject enemyObject, float delay)
    {
        if(enemyPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
        {
            await Task.Delay((int)(delay * 1000)); //MEMO : 유니티에서 async/await 비동기 작업하는게 비표준이긴 한것 같은데... 여기서 잠깐 기다리자고 코루틴 써야하나...????? 끔찍한레거시가되...
            pool.Release(enemyObject); //프리팹에 맞는 풀을 찾아서 릴리즈
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: {prefab.name}에 맞는 풀을 찾지 못하여 직접 삭제합니다");
            Destroy(enemyObject);
        }
    }

    private void OnApplicationQuit()
    {
        foreach(var pool in enemyPools.Values)
        {
            pool.Dispose();
        }
        enemyPools.Clear();
    }

    //초기화
    private void Awake()
    {
        _timeManager = FindFirstObjectByType<TimeManager>();

        //오브젝트 풀 초기화
        foreach(var wave in spawnWaves)
        {
            if(wave.monsterPrefab != null && !enemyPools.ContainsKey(wave.monsterPrefab))
            {
                //몬스터 프리팹에 해당하는 풀이 존재하지 않을 경우 새로 풀 생성
                enemyPools.Add(wave.monsterPrefab, new ObjectPool<GameObject>(
                    createFunc:
                    () => Instantiate(wave.monsterPrefab),
                    actionOnGet:
                    (obj) =>
                    {
                        obj.SetActive(true);
                        Enemy enemy = obj.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.ResetEnemyState();
                        }
                    },
                    actionOnRelease:
                    (obj) =>
                    {
                        obj.SetActive(false);
                    },
                    actionOnDestroy:
                    (obj) =>
                    {
                        Destroy(obj);
                    },
                    defaultCapacity: 100,
                    maxSize: 300
                    ));
            }
        }
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

        GameObject mobInstance = null;

        //object pooling
        if(enemyPools.TryGetValue(monsterPrefab, out ObjectPool<GameObject> pool))
        {
            mobInstance = pool.Get();
            mobInstance.transform.position = spawnPos;
            mobInstance.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError($"EnemySpawner: {monsterPrefab.name}에 대한 오브젝트 풀이 존재하지 않아 직접 생성합니다.");
            mobInstance = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
        }

        EnemyAI mobAI = mobInstance.GetComponent<EnemyAI>();
        Enemy mob = mobInstance.GetComponent<Enemy>();
        if(mobAI != null && mob != null)
        {
            mobAI.PlayerTransform = playerTransform;
            mob.OriginalPrefab = monsterPrefab;
        }
        return mobInstance;
    }
}