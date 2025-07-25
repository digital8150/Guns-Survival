using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ų�� ���ӿ��� ����ϱ� ���� ��� �� ����
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