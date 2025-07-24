using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 주변 적 틱뎀(뱀서의 마늘) 스킬의 스킬데이터
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
        Debug.LogWarning($"유효하지 않은 레벨 데이터! 스킬 이름 : {skillName}, 스킬 레벨 : {level}");
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
    public float damagePerTick; //틱당 데미지
    public float tickInterval; //틱 인터벌
    public float radius; //데미지 범위
}