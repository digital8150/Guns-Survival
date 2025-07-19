using UnityEngine;
using System.Collections; // IEnumerator ����� ���� �ʿ�

public class HealthPackSpawner : MonoBehaviour
{
    [Header("���� ���� ����")]
    [SerializeField]
    private GameObject healthPackPrefab;     // ������ ���� ������
    [SerializeField]
    private float SpawnDelay = 3f;           // ���� ������
    [SerializeField]
    private float respawnTime = 100f;        //������ ����� �� �ٽ� �����Ǳ������ �ֱ�

    private GameObject currentHealthPack; // ���� ������ ���� �ν��Ͻ��� ����

    void Start()
    {
        StartCoroutine(SpawnHealthPackRoutine(SpawnDelay));
    }

    //-------------------------------- ���� ���� ������ ���� --------------------------------------
    IEnumerator SpawnHealthPackRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);     //������

        // ������ ���� �����Ǿ� ���� ���� ���� ����
        if (currentHealthPack == null)
        {
            SpawnHealthPack();
        }
    }

    //-------------------------------------- ���� ���� ----------------------------------
    void SpawnHealthPack()
    {
        // ������ ������Ʈ�� ��ġ�� ���� ������ �ν��Ͻ�ȭ
        currentHealthPack = Instantiate(healthPackPrefab, transform.position, Quaternion.identity);
    }
}