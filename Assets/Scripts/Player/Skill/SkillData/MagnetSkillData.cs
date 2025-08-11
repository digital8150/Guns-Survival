using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ֺ� ����ġ ������� ��ų ������
/// </summary>
[CreateAssetMenu(fileName = "NewAreaDamageSkill", menuName = "Game Data/Skills/Magnet Skill")]
public class MagnetSkillData : SkillData
{
    public List<MagnetSkillLevelData> magnetLevels = new List<MagnetSkillLevelData>();
    public override int MaxLevel => magnetLevels.Count;

    public MagnetSkillLevelData GetLevelInfo(int level)
    {
        if (level > 0 && level <= MaxLevel)
        {
            return magnetLevels[level - 1];
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
public class MagnetSkillLevelData : SkillLevelInfo
{
    public float radius; //���� ����
}
