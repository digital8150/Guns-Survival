using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 보유하고 있는 스킬을 관리 (인벤토리)
/// 각 스킬을 실행 (폴리모피즘 적용)
/// </summary>
public class SkillManager : MonoBehaviour
{
    //TODO : 플레이어 스텟 관리 시스템 구축 및 연동 필요

    //플레이어가 보유한 스킬 목록과 스킬의 레벨
    private Dictionary<SkillData, int> ownedSkills = new Dictionary<SkillData, int>();

    //액티브 및 유틸리티 스킬 컨트롤러
    private Dictionary<SkillData, SkillController> activeSkillControllers = new Dictionary<SkillData, SkillController>();



    /// <summary>
    /// 스킬 획득 또는 업그레이드
    /// </summary>
    public void LearnOrUpgradeSkill(SkillData skillData)
    {
        //스킬을 보유하고 있지 않은 경우 레벨 0
        int currentLevel = 0;
        if (ownedSkills.ContainsKey(skillData))
        {
            //스킬을 보유하고 있는 경우 현재 스킬 레벨 가져오기
            currentLevel = ownedSkills[skillData];
        }

        if (currentLevel >= skillData.MaxLevel)
        {
            Debug.LogWarning($"스킬 {skillData.skillName}은 현재 이미 최대 레벨임!");
            return;
        }

        int newLevel = currentLevel + 1;
        ownedSkills[skillData] = newLevel;

        //스킬의 타입에 따라 처리
        //플레이어 스탯 증가형 스킬인 경우 :
        if (skillData is PlayerStatSkillData playerStatSkill)
        {
            //여기서 플레이어 스텟을 증가
            //TODO : 플레이어 스탯 관리 시스템 호출
            return;
        }

        //액티브 및 유틸형 스킬인 경우
        if (skillData is AreaDamageSkillData areaDamageSkillData)
        {
            if (!activeSkillControllers.ContainsKey(areaDamageSkillData))
            {
                //스킬이 처음 획득된 경우 해당 스킬 컨트롤러를 컴포넌트에 부착
                AreaDamageSkillController controller = gameObject.AddComponent<AreaDamageSkillController>();
                controller.skillData = areaDamageSkillData;
                //컨트롤러 딕셔너리에 등록
                activeSkillControllers.Add(areaDamageSkillData, controller);
            }
            (activeSkillControllers[areaDamageSkillData] as AreaDamageSkillController)?.UpdateSkillLevel(newLevel);
        }

    }

    //게임 테스트
    [Header("테스트!")]
    public SkillData testSkillData;

    private void Start()
    {
        LearnOrUpgradeSkill(testSkillData);
    }
}
