using System;
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
    public float MaxExp => maxExp;
    [SerializeField]
    private float nextLevelPenalty = 1.0f;     //���� ����ġ�� �ִ뷮 ����
    private float expGainMult = 1.0f; // ����ġ ȹ�� ����
    private SkillManager skillManager; //��ų�Ŵ��� ������Ʈ

    public static event Action<int> OnLevelChanged;
    public static event Action<float, float> OnExpChanged;



    private void Start()
    {
        skillManager = GetComponent<SkillManager>();
        OnExpChanged?.Invoke(currentExp, maxExp);
        OnLevelChanged?.Invoke(level);
    }

    //---------------����ġ ȹ�� ���� ����
    public void SetExpGainMult(float expGainMult)
    {
        this.expGainMult = expGainMult;
    }

    //---------------����ġ�� maxExp�� ������ ��������Ŵ----------------
    public void AddExp(float exp)
    {
        currentExp += exp * expGainMult; //����ġ ȹ�� ���� �ݿ�
        Debug.Log($"����ġ {exp} * {expGainMult} = {exp * expGainMult} ��ŭ ȹ��!");

        //������ �ѹ��� ������ ��Ȳ�� ����� ������ while�� ����
        while (currentExp >= maxExp)
        {
            LevelUp();
        }

        OnExpChanged?.Invoke(currentExp, maxExp);
    }

    //---------------------------- ������ ---------------------------
    public void LevelUp()
    {
        level++;                    //���� + 1
        currentExp -= maxExp;       //�ܿ� ����ġ �̵�
        maxExp *= nextLevelPenalty; //���� ������ �ʿ��� ����ġ�� �ش� ������ŭ ����
        
        skillManager.LevelUp();//��ų�Ŵ��� ������Ʈ�� ������ ȣ��

        OnLevelChanged?.Invoke(level);
        OnExpChanged?.Invoke(currentExp, maxExp);
    }
}
