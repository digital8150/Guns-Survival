using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// �ֺ� �� ƽ��(�켭�� ����) ��ų�� ��ų������
/// </summary>
[CreateAssetMenu(fileName = "NewAreaDamageSkill", menuName = "Game Data/Skills/Area Damage Skill")]
public class AreaDamageSkillData : SkillData
{
    public List<AreaDamageSkillLevelData> areaDamageLevels = new List<AreaDamageSkillLevelData>();

    public override int MaxLevel =>  areaDamageLevels.Count;

    public AreaDamageSkillLevelData GetLevelInfo(int level)
    {
        if (level > 0 && level <= MaxLevel)
        {
            return areaDamageLevels[level - 1];
        }
        Debug.LogWarning($"��ȿ���� ���� ���� ������! ��ų �̸� : {skillName}, ��ų ���� : {level}");
        return null;
    }

    public override SkillLevelInfo GetGenericLevelInfo(int level)
    {
        return GetLevelInfo(level);
    }
}

[System.Serializable]
public class AreaDamageSkillLevelData : SkillLevelInfo
{
    public float damagePerTick; //ƽ�� ������
    public float tickInterval; //ƽ ���͹�
    public float radius; //������ ����
}