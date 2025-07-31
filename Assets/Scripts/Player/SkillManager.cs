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
/// �÷��̾ �����ϰ� �ִ� ��ų�� ���� (�κ��丮)
/// �� ��ų�� ���� (���������� ����)
/// </summary>
/// 
public class SkillManager : MonoBehaviour
{
    //�ʿ� ������Ʈ
    private PlayerStatsManager playerStatsManager;
    private SkillDatabase skillDatabase;

    //�÷��̾ ������ ��ų ��ϰ� ��ų�� ����
    [Header("�÷��̾ ������ ��ų ���")]
    [SerializeField] //�ν����Ϳ��� ���� ���� ����ȭ 
    private SerializableDictionary<SkillData, int> ownedSkills = new SerializableDictionary<SkillData, int>();
    [SerializeField]
    private int maxSkillCount = 3; //���� ���� �ִ� ��ų ����

    //��Ƽ�� �� ��ƿ��Ƽ ��ų ��Ʈ�ѷ�
    private Dictionary<SkillData, SkillController> activeSkillControllers = new Dictionary<SkillData, SkillController>();

    private void Start()
    {
        playerStatsManager = GetComponent<PlayerStatsManager>();
        skillDatabase = FindFirstObjectByType<SkillDatabase>();
    }

    private void OnEnable()
    {
        EXPManager.OnLevelUp += LevelUp;
    }

    private void OnDisable()
    {
        EXPManager.OnLevelUp -= LevelUp;
    }

    /// <summary>
    /// ������ �� ���� ��ų ȹ�� �Ǵ� ���׷��̵�
    /// </summary>
    public void LevelUp()
    {
        if (skillDatabase != null)
        {

            //�ӽ� �׽�Ʈ ���� : �������� ��ų ���� �Ǵ� ���׷��̵�
            //TODO : ���� ���� ���������� ���� ������ �ý����� �����ؾ� ��.
            if(ownedSkills.Count < maxSkillCount)
            {
                //�ִ� ���� ��ų���� ���� ��ų�� ���� ���� ���
                //���� ���ӿ����� ������ ������ ���� �����ؾ� ��
                List<SkillData> cadinates = new List<SkillData>();
                foreach(var entry in skillDatabase.skillDatas) //�ĺ��� ��ų �����ͺ��̽��� ��� ��ų �߿��� �����´�
                {
                    if ( !ownedSkills.ContainsKey(entry.skillData) || entry.skillData.MaxLevel > ownedSkills[entry.skillData])
                    {
                        cadinates.Add(entry.skillData); //�ִ� ������ �ƴ� �͸� �ĺ��� ���
                    }
                }
                if (cadinates.Count == 0)
                {
                    Debug.Log("��� ��ų�� ���� �� �߽��ϴ�.");
                    return;
                }
                LearnOrUpgradeSkill(cadinates[UnityEngine.Random.Range(0, cadinates.Count)]); //�ĺ����� ȹ�� �Ǵ� ������
            }
            else
            {
                //�ִ� ���� ��ų�� ��� ������ ������ ���� �ִ� �� �߿��� ������
                List<SkillData> cadinates = new List<SkillData>();
                foreach (var entry in ownedSkills) //�ĺ��� �����ϰ� �ִ� ��ų ��Ͽ��� �����´�
                {
                    if(entry.Key.MaxLevel > entry.Value)
                    {
                        cadinates.Add(entry.Key); //�ִ� ������ �ƴ� �͸� �ĺ��� ���
                    }
                }

                if (cadinates.Count == 0)
                {
                    Debug.Log("��� ��ų�� ���� �� �߽��ϴ�.");
                    return;
                }
                LearnOrUpgradeSkill(cadinates[UnityEngine.Random.Range(0, cadinates.Count)]); //�ĺ����� ������
            }
        }
        else
        {
            Debug.LogError("��ų �����ͺ��̽��� null�̾����ϴ�.");
        }
    }

    /// <summary>
    /// ��ų ȹ�� �Ǵ� ���׷��̵�
    /// </summary>
    public void LearnOrUpgradeSkill(SkillData skillData)
    {
        //��ų�� �����ϰ� ���� ���� ��� ���� 0
        int currentLevel = 0;
        if (ownedSkills.ContainsKey(skillData))
        {
            //��ų�� �����ϰ� �ִ� ��� ���� ��ų ���� ��������
            currentLevel = ownedSkills[skillData];
        }

        if (currentLevel >= skillData.MaxLevel)
        {
            Debug.LogWarning($"��ų {skillData.skillName}�� ���� �̹� �ִ� ������!");
            return;
        }

        int newLevel = currentLevel + 1;
        ownedSkills[skillData] = newLevel;

        //��ų�� Ÿ�Կ� ���� ó��
        //�÷��̾� ���� ������ ��ų�� ��� :
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
                    Debug.LogWarning("������ �ӵ� ������ ���� ���� �ʿ�");
                    break;
                default:
                    throw new NotImplementedException($"���� ���� ���� PlayerStatType�� ���� ó�� �߻� : {playerStatSkill.statType}");
            }
            Debug.Log($"��ų ������(�Ǵ� ����)! : {skillData.skillName} | {skillData.description} | {skillData.GetGenericLevelInfo(newLevel).upgradeDescription}");
            return;
        }

        //��Ƽ�� �� ��ƿ�� ��ų�� ���
        if (skillData is AreaDamageSkillData areaDamageSkillData)
        {
            if (!activeSkillControllers.ContainsKey(areaDamageSkillData))
            {
                //��ų�� ó�� ȹ��� ��� �ش� ��ų ��Ʈ�ѷ��� ������Ʈ�� ����
                AreaDamageSkillController controller = gameObject.AddComponent<AreaDamageSkillController>();
                controller.skillData = areaDamageSkillData;
                //��Ʈ�ѷ� ��ųʸ��� ���
                activeSkillControllers.Add(areaDamageSkillData, controller);
            }
            (activeSkillControllers[areaDamageSkillData] as AreaDamageSkillController)?.UpdateSkillLevel(newLevel);
            Debug.Log($"��ų ������(�Ǵ� ����)! : {skillData.skillName} | {skillData.description} | {skillData.GetGenericLevelInfo(newLevel).upgradeDescription}");
        }

    }


}
