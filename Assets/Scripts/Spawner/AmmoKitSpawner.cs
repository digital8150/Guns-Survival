using UnityEngine;
using System.Collections;

public class AmmoPackSpawner : MonoBehaviour
{
    [Header("탄창 스폰 설정")]
    [SerializeField]
    private GameObject ammoPackPrefab;          // 스폰할 탄창 프리팹
    [SerializeField]
    private float SpawnDelay = 3f;              // 스폰 딜레이
    [SerializeField]
    private float respawnTime = 50f;           //탄창이 사라진 후 다시 스폰되기까지의 주기

    private GameObject currentAmmoPack;       // 현재 스폰된 탄창 인스턴스를 추적

    void Start()
    {
        StartCoroutine(SpawnAmmoPackCoroutine(SpawnDelay));
    }

    //-------------------------------- 탄창 스폰 딜레이 관리 --------------------------------------
    IEnumerator SpawnAmmoPackCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);     //딜레이

        // 탄창이 현재 스폰되어 있지 않을 때만 스폰
        if (currentAmmoPack == null)
        {
            SpawnAmmoPack();
        }
    }

    //-------------------------------------- 탄창 생성 ----------------------------------
    void SpawnAmmoPack()
    {
        // 스포너 오브젝트의 위치에 탄창 프리팹 인스턴스화
        currentAmmoPack = Instantiate(ammoPackPrefab, transform.position, Quaternion.identity);
    }
}
