using UnityEngine;

public class MagnetSkillController : SkillController
{
    private float tickTimer;
    private Collider[] hitBuffer = new Collider[150];

    //원기둥 판정 높이 상수
    private const float CAPSULEHEIGHT = 15f;
    private const float TICKTIME = 0.2f;
    private const float EXPATTRACTSPEED = 10f;

    public override void UpdateSkillLevel(int newlevel)
    {
        this.currentLevel = newlevel;
    }

    private void Update()
    {
        if (skillData == null || currentLevel == 0) return;

        MagnetSkillData magnetSkillData = skillData as MagnetSkillData;
        MagnetSkillLevelData currentLevelData = magnetSkillData.GetLevelInfo(currentLevel);

        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            PerformMagnet(currentLevelData);
            tickTimer = TICKTIME;
        }
    }

    void PerformMagnet (MagnetSkillLevelData levelData)
    {
        float capsuleHalfHeight = CAPSULEHEIGHT / 2;
        Vector3 bottomOfCapsule = transform.position - (Vector3.up * capsuleHalfHeight);
        Vector3 topOfCapsule = transform.position + (Vector3.up * capsuleHalfHeight);

        int numColliders = Physics.OverlapCapsuleNonAlloc(bottomOfCapsule, topOfCapsule, levelData.radius, hitBuffer);

        for (int i = 0; i < numColliders; i++)
        {
            Collider hit = hitBuffer[i];
            if (hit.CompareTag("EXP"))
            {
                SetupEXPMovement(hit);
            }
        }
    }

    private void SetupEXPMovement(Collider hit)
    {
        EXP exp = hit.GetComponent<EXP>();
        if (exp != null)
        {
            exp.Movement3D.MoveSpeed = EXPATTRACTSPEED;
            exp.MoveTo.Setup(gameObject.transform);
        }
    }
}
