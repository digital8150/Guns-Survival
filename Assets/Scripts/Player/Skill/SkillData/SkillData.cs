using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ų ������ ���̽� Ŭ����
/// </summary>
public abstract class SkillData : ScriptableObject
{
    public string skillName; //��ų �̸�
    public string description; //��ų ����
    public Sprite skillIcon; //���� UI ���

    public abstract int MaxLevel { get; }
    public abstract SkillLevelInfo GetGenericLevelInfo(int level);
}

[System.Serializable]
public class SkillLevelInfo
{
    public int level; //����
    public string upgradeDescription; //���׷��̵� ���� (UI ���)
}
