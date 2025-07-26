using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();


    [SerializeField]
    private List<TValue> values = new List<TValue>();


    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }


    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();


        if (keys.Count != values.Count)
            throw new Exception("there are " + keys.Count + " keys and " + values.Count + " values after deserialization. Make sure that both key and value types are serializable.");


        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}

/// <summary>
/// 플레이어가 보유하고 있는 스킬을 관리 (인벤토리)
/// 각 스킬을 실행 (폴리모피즘 적용)
/// </summary>
/// 
public class SkillManager : MonoBehaviour
{
    //필요 컴포넌트
    private PlayerStatsManager playerStatsManager;
    private SkillDatabase skillDatabase;

    //플레이어가 보유한 스킬 목록과 스킬의 레벨
    [Header("플레이어가 보유한 스킬 목록")]
    [SerializeField] //인스펙터에서 보기 위해 직렬화 
    private SerializableDictionary<SkillData, int> ownedSkills = new SerializableDictionary<SkillData, int>();
    [SerializeField]
    private int maxSkillCount = 3; //보유 가능 최대 스킬 개수

    //액티브 및 유틸리티 스킬 컨트롤러
    private Dictionary<SkillData, SkillController> activeSkillControllers = new Dictionary<SkillData, SkillController>();

    private void Start()
    {
        playerStatsManager = GetComponent<PlayerStatsManager>();
        skillDatabase = FindFirstObjectByType<SkillDatabase>();
    }

    /// <summary>
    /// 레벨업 시 랜덤 스킬 획득 또는 업그레이드
    /// </summary>
    public void LevelUp()
    {
        if (skillDatabase != null)
        {

            //임시 테스트 구현 : 랜덤으로 스킬 슥듭 또는 업그레이드
            //TODO : 실제 게임 구현에서는 증강 선택지 시스템을 구현해야 함.
            if(ownedSkills.Count < maxSkillCount)
            {
                //최대 보유 스킬보다 적은 스킬을 보유 중인 경우
                //실제 게임에서는 레벨업 선택지 제공 구현해야 함
                List<SkillData> cadinates = new List<SkillData>();
                foreach(var entry in skillDatabase.skillDatas) //후보는 스킬 데이터베이스의 모든 스킬 중에서 가져온다
                {
                    if ( !ownedSkills.ContainsKey(entry.skillData) || entry.skillData.MaxLevel > ownedSkills[entry.skillData])
                    {
                        cadinates.Add(entry.skillData); //최대 레벨이 아닌 것만 후보에 등록
                    }
                }
                if (cadinates.Count == 0)
                {
                    Debug.Log("모든 스킬을 레벨 업 했습니다.");
                    return;
                }
                LearnOrUpgradeSkill(cadinates[UnityEngine.Random.Range(0, cadinates.Count)]); //후보에서 획득 또는 레벨업
            }
            else
            {
                //최대 보유 스킬을 모두 가지고 있으면 갖고 있는 것 중에서 레벨업
                List<SkillData> cadinates = new List<SkillData>();
                foreach (var entry in ownedSkills) //후보는 보유하고 있는 스킬 목록에서 가져온다
                {
                    if(entry.Key.MaxLevel > entry.Value)
                    {
                        cadinates.Add(entry.Key); //최대 레벨이 아닌 것만 후보에 등록
                    }
                }

                if (cadinates.Count == 0)
                {
                    Debug.Log("모든 스킬을 레벨 업 했습니다.");
                    return;
                }
                LearnOrUpgradeSkill(cadinates[UnityEngine.Random.Range(0, cadinates.Count)]); //후보에서 레벨업
            }
        }
        else
        {
            Debug.LogError("스킬 데이터베이스가 null이었습니다.");
        }
    }

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
            switch (playerStatSkill.statType)
            {
                case PlayerStatType.MaxHealth:
                    playerStatsManager.IncreaseMaxHp(playerStatSkill.GetLevelInfo(newLevel).value);
                    break;
                case PlayerStatType.MovementSpeed:
                    playerStatsManager.IncreaseMovementSpeed(playerStatSkill.GetLevelInfo(newLevel).value);
                    break;
                case PlayerStatType.ExperienceGainMult:
                    playerStatsManager.SetExpGainMult(playerStatSkill.GetLevelInfo(newLevel).value);
                    break;
                case PlayerStatType.ReloadSpeedMult:
                    Debug.LogWarning("재장전 속도 증가는 현재 구현 필요");
                    break;
                default:
                    throw new NotImplementedException($"구현 되지 않은 PlayerStatType에 대한 처리 발생 : {playerStatSkill.statType}");
            }
            Debug.Log($"스킬 레벨업(또는 습득)! : {skillData.skillName} | {skillData.description} | {skillData.GetGenericLevelInfo(newLevel).upgradeDescription}");
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
            Debug.Log($"스킬 레벨업(또는 습득)! : {skillData.skillName} | {skillData.description} | {skillData.GetGenericLevelInfo(newLevel).upgradeDescription}");
        }

    }


}
