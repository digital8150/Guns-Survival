using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 능력치 강화형 패시브 스킬을 위한 스킬 데이터
/// </summary>
[CreateAssetMenu(fileName = "NewPlayerStatSkill", menuName = "Game Data/Skills/Player Stat Skill")]
public class PlayerStatSkillData : SkillData
{
    public PlayerStatType statType; //어떤 플레이어의 스탯을 강화할까

    public List<PlayerStatSkillLevelData> playerStatLevels = new List<PlayerStatSkillLevelData>();

    public override int MaxLevel => playerStatLevels.Count;

    public PlayerStatSkillLevelData GetLevelInfo(int level)
    {
        if (level > 0 && level <= MaxLevel)
        {
            return playerStatLevels[level - 1 ];
        }
        Debug.LogWarning($"유효하지 않은 레벨 데이터! 스킬 이름 : {skillName}, 스킬 레벨 : {level}");
        return null;
    }

    public override SkillLevelInfo GetGenericLevelInfo(int level)
    {
        return GetLevelInfo(level);
    }
}

public enum PlayerStatType
{
    ExperienceGainMult, //획득 경험치량 배수 (곱연산)
    MaxHealth, //최대 체력 (합연산)
    MovementSpeed, //이동 속도 (합연산)
    ReloadSpeedMult //장전 속도 배수 (곱연산)
}

[System.Serializable]
public class PlayerStatSkillLevelData : SkillLevelInfo
{
    public float value; //해당 레벨에서 스탯에 적용할 값 (증가량 or 배율)
}