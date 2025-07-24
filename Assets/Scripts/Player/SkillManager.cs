using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ �����ϰ� �ִ� ��ų�� ���� (�κ��丮)
/// �� ��ų�� ���� (���������� ����)
/// </summary>
public class SkillManager : MonoBehaviour
{
    //TODO : �÷��̾� ���� ���� �ý��� ���� �� ���� �ʿ�

    //�÷��̾ ������ ��ų ��ϰ� ��ų�� ����
    private Dictionary<SkillData, int> ownedSkills = new Dictionary<SkillData, int>();

    //��Ƽ�� �� ��ƿ��Ƽ ��ų ��Ʈ�ѷ�
    private Dictionary<SkillData, SkillController> activeSkillControllers = new Dictionary<SkillData, SkillController>();



    /// <summary>
    /// ��ų ȹ�� �Ǵ� ���׷��̵�
    /// </summary>
    public void LearnOrUpgradeSkill(SkillData skillData)
    {
        //��ų�� �����ϰ� ���� ���� ��� ���� 0
        int currentLevel = 0;
        if (ownedSkills.ContainsKey(skillData))
        {
            //��ų�� �����ϰ� �ִ� ��� ���� ��ų ���� ��������
            currentLevel = ownedSkills[skillData];
        }

        if (currentLevel >= skillData.MaxLevel)
        {
            Debug.LogWarning($"��ų {skillData.skillName}�� ���� �̹� �ִ� ������!");
            return;
        }

        int newLevel = currentLevel + 1;
        ownedSkills[skillData] = newLevel;

        //��ų�� Ÿ�Կ� ���� ó��
        //�÷��̾� ���� ������ ��ų�� ��� :
        if (skillData is PlayerStatSkillData playerStatSkill)
        {
            //���⼭ �÷��̾� ������ ����
            //TODO : �÷��̾� ���� ���� �ý��� ȣ��
            return;
        }

        //��Ƽ�� �� ��ƿ�� ��ų�� ���
        if (skillData is AreaDamageSkillData areaDamageSkillData)
        {
            if (!activeSkillControllers.ContainsKey(areaDamageSkillData))
            {
                //��ų�� ó�� ȹ��� ��� �ش� ��ų ��Ʈ�ѷ��� ������Ʈ�� ����
                AreaDamageSkillController controller = gameObject.AddComponent<AreaDamageSkillController>();
                controller.skillData = areaDamageSkillData;
                //��Ʈ�ѷ� ��ųʸ��� ���
                activeSkillControllers.Add(areaDamageSkillData, controller);
            }
            (activeSkillControllers[areaDamageSkillData] as AreaDamageSkillController)?.UpdateSkillLevel(newLevel);
        }

    }

    //���� �׽�Ʈ
    [Header("�׽�Ʈ!")]
    public SkillData testSkillData;

    private void Start()
    {
        LearnOrUpgradeSkill(testSkillData);
    }
}
