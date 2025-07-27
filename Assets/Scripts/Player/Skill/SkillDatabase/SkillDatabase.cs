using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬을 게임에서 사용하기 위해 등록 및 관리
/// </summary>
public class SkillDatabase : MonoBehaviour
{
    public List<SkillEntry> skillDatas;
    public Dictionary<string, SkillData> SkillDictionary = new Dictionary<string, SkillData>();

    private void Start()
    {
        foreach(var entry in skillDatas)
        {
            SkillDictionary.Add(entry.id, entry.skillData);
        }
    }


}

[System.Serializable]
public class SkillEntry
{
    public string id;
    public SkillData skillData;
}