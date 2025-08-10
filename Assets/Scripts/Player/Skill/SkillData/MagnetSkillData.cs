using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 주변 경험치 끌어당기기 스킬 데이터
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
        Debug.LogWarning($"유효하지 않은 레벨 데이터! 스킬 이름 : {skillName}, 스킬 레벨 : {level}");
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
    public float radius; //적용 범위
}
