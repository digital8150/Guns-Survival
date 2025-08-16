using System.Collections;
using DTT.AreaOfEffectRegions;
using UnityEngine;

public class AreaDamageSkillController : SkillController
{
    private float tickTimer;
    private Collider[] hitBuffer = new Collider[200];

    //원기둥 판정 높이 상수
    private const float CAPSULEHEIGHT = 50f;
    private const float LERPDURATION = 0.25f;

    //레퍼런스
    private CircleRegionProjector skillRegionIndicator;

    private void Awake()
    {
        skillRegionIndicator = GetComponentInChildren<CircleRegionProjector>();
    }

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
        UpdateIndicatorRadius();
    }

    void UpdateIndicatorRadius()
    {
        if(skillData == null || currentLevel <= 0 || skillRegionIndicator == null)
        {
            return;
        }

        AreaDamageSkillData areaDamageSkillData = skillData as AreaDamageSkillData;
        StartCoroutine(LerpRadius(skillRegionIndicator.Radius, areaDamageSkillData.GetLevelInfo(currentLevel).radius));

    }

    IEnumerator LerpRadius(float origin, float target)
    {
        float elapsedTime = 0f;
        while (elapsedTime < LERPDURATION)
        {
            skillRegionIndicator.Radius = Mathf.Lerp(origin, target, elapsedTime / LERPDURATION);
            skillRegionIndicator.UpdateProjectors();
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private void PerformAreaDamage(AreaDamageSkillLevelData levelData)
    {
        float capsuleHalfHeight = CAPSULEHEIGHT / 2;
        Vector3 bottomOfCapsule = transform.position - (Vector3.up * capsuleHalfHeight);
        Vector3 topOfCapsule = transform.position + (Vector3.up * capsuleHalfHeight);

        int numColliders = Physics.OverlapCapsuleNonAlloc(bottomOfCapsule, topOfCapsule, levelData.radius, hitBuffer);
        
        for (int i = 0; i < numColliders; i++)
        {
            Collider hit = hitBuffer[i];
            if (hit.CompareTag("Enemy") || hit.CompareTag("Boss") || hit.CompareTag("FinalBoss"))
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Damage(levelData.damagePerTick);
                    Debug.Log($"마늘과 고추 공격 ({levelData.damagePerTick}) -> ({enemy.name})");
                }
            }
        }
    }
}