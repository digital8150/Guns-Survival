using UnityEngine;

public abstract class SkillController : MonoBehaviour
{
    public SkillData skillData;
    protected int currentLevel;

    public abstract void UpdateSkillLevel(int newlevel);
}
