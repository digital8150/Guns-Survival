using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �ϴ� ���Ͱ� ���̺� ���� ������� ������ �͸� ����
/// ���Ŀ� ���� ü���� ���� �þ �����ȴٴ����� ���߿� ����
/// </summary>

//���̺� ����
[System.Serializable]
public class Wave
{
    public string waveName;                         // ���̺� �̸�
    public float waveStartDelay = 0f;               // ���� ���̺� ���� �� �� ���̺� ���۱����� ��� �ð�
    public List<MonsterGroup> monsterGroups;        // �� ���̺꿡�� ������ ���� �׷� ���
    public float nextWaveDelay = 5f;                // �� ���̺갡 ���� �� ���� ���̺� ���۱��� ��� �ð�
}

//���̺� ����
[System.Serializable]
public class MonsterGroup
{
    public GameObject monsterPrefab;                // ������ ���� ������
    public int numberOfMonsters = 1;                // ������ ���� ��
    public float spawnInterval = 1f;                // �� �׷� ������ �� ���Ͱ� �����Ǵ� ����
}

public class EnemySpawner : MonoBehaviour
{
    [Header("���� ����")]
    [SerializeField]
    private float spawnRadius = 3f;       // ������ ��ġ�κ��� ���Ͱ� ������ �ݰ�

    [Header("���̺� ����")]
    [SerializeField]
    private Wave[] waves; // �� ���̺��� ������ ��� �迭

    private int currentWaveIndex = 0; // ���� ���� ���� ���̺� �ε���
    private bool isSpawning = false; // ���� ���� ���� ������ ����

    //�ʱ�ȭ
    void Start()
    {
        StartCoroutine(StartWavesCoroutine());
    }

    //-------------------------------------- ���̺� ���� ----------------------------------------
    IEnumerator StartWavesCoroutine()
    {
        yield return new WaitForSeconds(3f); // ���� ���� �� ù ���̺���� ��� �ð�

        Debug.Log("���� ���� ����");

        // ��� ���̺긦 ������� ����
        for (currentWaveIndex = 0; currentWaveIndex < waves.Length; currentWaveIndex++)
        {
            Wave currentWave = waves[currentWaveIndex];

            // ���� ���̺���� ��ٸ� �ð� (���� ���̺갡 ������ ���� ���̺� ���۱����� ������)
            if (currentWaveIndex > 0) // ù ���̺갡 �ƴ� ��쿡�� ����
                yield return new WaitForSeconds(currentWave.waveStartDelay);

            // ���� ���̺��� ��� ���� �׷��� ��ȸ�ϸ� ����
            foreach (MonsterGroup group in currentWave.monsterGroups)
            {
                if (group.monsterPrefab == null)
                    continue;

                // �ش� ���� �׷��� ���͵��� ����
                for (int i = 0; i < group.numberOfMonsters; i++)
                {
                    SpawnMonster(group.monsterPrefab);
                    yield return new WaitForSeconds(group.spawnInterval); // ���� ���� �������� ���
                }
            }
            yield return new WaitForSeconds(currentWave.nextWaveDelay); // ���� ���̺� ���� �� ���� ���̺� ���۱��� ���
        }
    }

    //------------------------------------- ���� ���� ------------------------------------------
    void SpawnMonster(GameObject monsterPrefab)
    {
        // ������ ������Ʈ�� ��ġ�� �������� ������ ������ ���� (X, Z ���)
        Vector2 randomCirclePoint = Random.insideUnitCircle * spawnRadius;
        Vector3 randomOffset = new Vector3(randomCirclePoint.x, 0f, randomCirclePoint.y);

        // ���� ���� ��ġ ���
        Vector3 spawnPosition = transform.position + randomOffset;

        // ���� ������ ����
        GameObject spawnedMonster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
    }
}