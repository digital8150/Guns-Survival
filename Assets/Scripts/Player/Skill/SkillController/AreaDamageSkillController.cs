using UnityEngine;

public class AreaDamageSkillController : SkillController
{
    private float tickTimer;
    private Collider[] hitBuffer = new Collider[50];

    private void Update()
    {
        if (skillData == null || currentLevel == 0) return;

        AreaDamageSkillData areaDamageSkillData = skillData as AreaDamageSkillData;
        AreaDamageSkillLevelData currentLevelData = areaDamageSkillData.GetLevelInfo(currentLevel);

        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            PerformAreaDamage(currentLevelData);
            tickTimer = currentLevelData.tickInterval;
        }
    }

    public override void UpdateSkillLevel(int newLevel)
    {
        this.currentLevel = newLevel;
        tickTimer = 0f;
    }

    private void PerformAreaDamage(AreaDamageSkillLevelData levelData)
    {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, levelData.radius, hitBuffer);
        for (int i = 0; i < numColliders; i++)
        {
            Collider hit = hitBuffer[i];
            if (hit.CompareTag("Enemy"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Damage(levelData.damagePerTick);
                    Debug.Log($"{hit.name}에게 {levelData.damagePerTick}의 틱 데미지 적용!");
                }
            }
        }
    }
}