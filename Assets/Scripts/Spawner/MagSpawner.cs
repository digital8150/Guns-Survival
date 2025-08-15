using UnityEngine;
using System.Collections;

public class MagSpawner : MonoBehaviour
{
    [Header("자석 스폰 설정")]
    [SerializeField]
    private GameObject MagPrefab;               //프리팹
    [SerializeField]
    private float SpawnDelay = 3f;              // 스폰 딜레이
    [SerializeField]
    private float respawnTime = 50f;           //다시 스폰되기까지의 주기

    private GameObject currentPrefab;       // 현재 인스턴스를 추적

    void Start()
    {
        StartCoroutine(SpawnMagCoroutine(SpawnDelay));
    }

    //-------------------------------- 딜레이 관리 --------------------------------------
    IEnumerator SpawnMagCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);     //딜레이

        if (currentPrefab == null)
        {
            SpawnMag();
        }
    }

    //-------------------------------------- 생성 ----------------------------------
    void SpawnMag()
    {
        // 스포너 오브젝트의 위치에 프리팹 인스턴스화
        currentPrefab = Instantiate(MagPrefab, transform.position, Quaternion.identity);
    }
}
