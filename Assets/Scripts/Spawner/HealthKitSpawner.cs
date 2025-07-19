using UnityEngine;
using System.Collections; // IEnumerator 사용을 위해 필요

public class HealthPackSpawner : MonoBehaviour
{
    [Header("힐팩 스폰 설정")]
    [SerializeField]
    private GameObject healthPackPrefab;     // 스폰할 힐팩 프리팹
    [SerializeField]
    private float SpawnDelay = 3f;           // 스폰 딜레이
    [SerializeField]
    private float respawnTime = 100f;        //힐팩이 사라진 후 다시 스폰되기까지의 주기

    private GameObject currentHealthPack; // 현재 스폰된 힐팩 인스턴스를 추적

    void Start()
    {
        StartCoroutine(SpawnHealthPackRoutine(SpawnDelay));
    }

    //-------------------------------- 힐팩 스폰 딜레이 관리 --------------------------------------
    IEnumerator SpawnHealthPackRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);     //딜레이

        // 힐팩이 현재 스폰되어 있지 않을 때만 스폰
        if (currentHealthPack == null)
        {
            SpawnHealthPack();
        }
    }

    //-------------------------------------- 힐팩 생성 ----------------------------------
    void SpawnHealthPack()
    {
        // 스포너 오브젝트의 위치에 힐팩 프리팹 인스턴스화
        currentHealthPack = Instantiate(healthPackPrefab, transform.position, Quaternion.identity);
    }
}