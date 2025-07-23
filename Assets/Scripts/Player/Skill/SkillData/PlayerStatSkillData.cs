using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾� �ɷ�ġ ��ȭ�� �нú� ��ų�� ���� ��ų ������
/// </summary>
[CreateAssetMenu(fileName = "NewPlayerStatSkill", menuName = "Game Data/Skills/Player Stat Skill")]
public class PlayerStatSkillData : SkillData
{
    public PlayerStatType statType; //� �÷��̾��� ������ ��ȭ�ұ�

    public List<PlayerStatSkillLevelData> playerStatLevels = new List<PlayerStatSkillLevelData>();

    public override int MaxLevel => playerStatLevels.Count;

    public PlayerStatSkillLevelData GetLevelInfo(int level)
    {
        if (level > 0 && level <= MaxLevel)
        {
            return playerStatLevels[level - 1 ];
        }
        Debug.LogWarning($"��ȿ���� ���� ���� ������! ��ų �̸� : {skillName}, ��ų ���� : {level}");
        return null;
    }

    public override SkillLevelInfo GetGenericLevelInfo(int level)
    {
        return GetLevelInfo(level);
    }
}

public enum PlayerStatType
{
    ExperienceGainMult, //ȹ�� ����ġ�� ��� (������)
    MaxHealth, //�ִ� ü�� (�տ���)
    MovementSpeed, //�̵� �ӵ� (�տ���)
    ReloadSpeedMult //���� �ӵ� ��� (������)
}

[System.Serializable]
public class PlayerStatSkillLevelData : SkillLevelInfo
{
    public float value; //�ش� �������� ���ȿ� ������ �� (������ or ����)
}