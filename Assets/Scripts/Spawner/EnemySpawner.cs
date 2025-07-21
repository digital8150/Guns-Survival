using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 일단 몬스터가 웨이브 별로 순서대로 나오는 것만 구현
/// 추후에 몬스터 체력이 점점 늘어서 스폰된다던지는 나중에 생각
/// </summary>

//웨이브 정보
[System.Serializable]
public class Wave
{
    public string waveName;                         // 웨이브 이름
    public float waveStartDelay = 0f;               // 이전 웨이브 종료 후 이 웨이브 시작까지의 대기 시간
    public List<MonsterGroup> monsterGroups;        // 이 웨이브에서 스폰할 몬스터 그룹 목록
    public float nextWaveDelay = 5f;                // 이 웨이브가 끝난 후 다음 웨이브 시작까지 대기 시간
}

//웨이브 설정
[System.Serializable]
public class MonsterGroup
{
    public GameObject monsterPrefab;                // 스폰할 몬스터 프리팹
    public int numberOfMonsters = 1;                // 스폰할 몬스터 수
    public float spawnInterval = 1f;                // 이 그룹 내에서 각 몬스터가 스폰되는 간격
}

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    [SerializeField]
    private float spawnRadius = 3f;       // 스포너 위치로부터 몬스터가 스폰될 반경

    [Header("웨이브 설정")]
    [SerializeField]
    private Wave[] waves; // 각 웨이브의 정보를 담는 배열

    private int currentWaveIndex = 0; // 현재 진행 중인 웨이브 인덱스
    private bool isSpawning = false; // 현재 몬스터 스폰 중인지 여부

    //초기화
    void Start()
    {
        StartCoroutine(StartWavesCoroutine());
    }

    //-------------------------------------- 웨이브 생성 ----------------------------------------
    IEnumerator StartWavesCoroutine()
    {
        yield return new WaitForSeconds(3f); // 게임 시작 후 첫 웨이브까지 대기 시간

        Debug.Log("몬스터 스폰 시작");

        // 모든 웨이브를 순서대로 진행
        for (currentWaveIndex = 0; currentWaveIndex < waves.Length; currentWaveIndex++)
        {
            Wave currentWave = waves[currentWaveIndex];

            // 다음 웨이브까지 기다릴 시간 (이전 웨이브가 끝나고 다음 웨이브 시작까지의 딜레이)
            if (currentWaveIndex > 0) // 첫 웨이브가 아닐 경우에만 적용
                yield return new WaitForSeconds(currentWave.waveStartDelay);

            // 현재 웨이브의 모든 몬스터 그룹을 순회하며 스폰
            foreach (MonsterGroup group in currentWave.monsterGroups)
            {
                if (group.monsterPrefab == null)
                    continue;

                // 해당 몬스터 그룹의 몬스터들을 스폰
                for (int i = 0; i < group.numberOfMonsters; i++)
                {
                    SpawnMonster(group.monsterPrefab);
                    yield return new WaitForSeconds(group.spawnInterval); // 다음 몬스터 스폰까지 대기
                }
            }
            yield return new WaitForSeconds(currentWave.nextWaveDelay); // 현재 웨이브 종료 후 다음 웨이브 시작까지 대기
        }
    }

    //------------------------------------- 몬스터 스폰 ------------------------------------------
    void SpawnMonster(GameObject monsterPrefab)
    {
        // 스포너 오브젝트의 위치를 기준으로 랜덤한 오프셋 생성 (X, Z 평면)
        Vector2 randomCirclePoint = Random.insideUnitCircle * spawnRadius;
        Vector3 randomOffset = new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

        // 최종 스폰 위치 계산
        Vector3 spawnPosition = transform.position + randomOffset;

        // 몬스터 프리팹 생성
        GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
    }
}