using UnityEngine;
using System.Collections;

public class MagSpawner : MonoBehaviour
{
    [Header("�ڼ� ���� ����")]
    [SerializeField]
    private GameObject MagPrefab;               //������
    [SerializeField]
    private float SpawnDelay = 3f;              // ���� ������
    [SerializeField]
    private float respawnTime = 50f;           //�ٽ� �����Ǳ������ �ֱ�

    private GameObject currentPrefab;       // ���� �ν��Ͻ��� ����

    void Start()
    {
        StartCoroutine(SpawnMagCoroutine(SpawnDelay));
    }

    //-------------------------------- ������ ���� --------------------------------------
    IEnumerator SpawnMagCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);     //������

        if (currentPrefab == null)
        {
            SpawnMag();
        }
    }

    //-------------------------------------- ���� ----------------------------------
    void SpawnMag()
    {
        // ������ ������Ʈ�� ��ġ�� ������ �ν��Ͻ�ȭ
        currentPrefab = Instantiate(MagPrefab, transform.position, Quaternion.identity);
    }
}
