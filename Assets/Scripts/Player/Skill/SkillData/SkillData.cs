using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 데이터 베이스 클래스
/// </summary>
public abstract class SkillData : ScriptableObject
{
    public string skillName; //스킬 이름
    public string description; //스킬 설명
    public Sprite skillIcon; //추후 UI 고려

    public abstract int MaxLevel { get; }
    public abstract SkillLevelInfo GetGenericLevelInfo(int level);
}

[System.Serializable]
public class SkillLevelInfo
{
    public int level; //레벨
    public string upgradeDescription; //업그레이드 설명 (UI 고려)
}
