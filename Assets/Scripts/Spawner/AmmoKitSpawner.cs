using UnityEngine;
using System.Collections;

public class AmmoPackSpawner : MonoBehaviour
{
    [Header("źâ ���� ����")]
    [SerializeField]
    private GameObject ammoPackPrefab;          // ������ źâ ������
    [SerializeField]
    private float SpawnDelay = 3f;              // ���� ������
    [SerializeField]
    private float respawnTime = 50f;           //źâ�� ����� �� �ٽ� �����Ǳ������ �ֱ�

    private GameObject currentAmmoPack;       // ���� ������ źâ �ν��Ͻ��� ����

    void Start()
    {
        StartCoroutine(SpawnAmmoPackCoroutine(SpawnDelay));
    }

    //-------------------------------- źâ ���� ������ ���� --------------------------------------
    IEnumerator SpawnAmmoPackCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);     //������

        // źâ�� ���� �����Ǿ� ���� ���� ���� ����
        if (currentAmmoPack == null)
        {
            SpawnAmmoPack();
        }
    }

    //-------------------------------------- źâ ���� ----------------------------------
    void SpawnAmmoPack()
    {
        // ������ ������Ʈ�� ��ġ�� źâ ������ �ν��Ͻ�ȭ
        currentAmmoPack = Instantiate(ammoPackPrefab, transform.position, Quaternion.identity);
    }
}
