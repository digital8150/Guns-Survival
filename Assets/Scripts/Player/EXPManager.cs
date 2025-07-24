using UnityEngine;

public class EXPManager : MonoBehaviour
{
    [Header("�÷��̾� ����ġ �� ���� ����")]
    [SerializeField]
    private int level = 1;              //���� ����
    [SerializeField]
    private float currentExp = 0;       //�ǽð� Ȯ�ο����� ����ȭ��Ŵ
    [SerializeField]
    private float maxExp = 100;         //�ִ� ����ġ
    [SerializeField]
    private float nextLevelPenalty = 1.0f;     //���� ����ġ�� �ִ뷮 ����

    //---------------����ġ�� maxExp�� ������ ��������Ŵ----------------
    public void AddExp(float exp)
    {
        currentExp += exp;
        Debug.Log($"����ġ {exp} ��ŭ ȹ��!");

        if (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    //---------------------------- ������ ---------------------------
    public void LevelUp()
    {
        level++;                    //���� + 1
        currentExp -= maxExp;       //�ܿ� ����ġ �̵�
        maxExp *= nextLevelPenalty; //���� ������ �ʿ��� ����ġ�� �ش� ������ŭ ����

        Debug.Log($"���� ��! ���� ���� {level}");
    }
}
