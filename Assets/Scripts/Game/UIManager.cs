using System.Collections.Generic;
using GameBuilders.MinimalistUI.Scripts;
using GameBuilders.FPSBuilder.Core.Player;
using GameBuilders.FPSBuilder.Core.Inventory;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Drawing.Printing;
using static UnityEngine.InputSystem.XR.TrackedPoseDriver;
using GameBuilders.FPSBuilder.Core.Weapons;

public class UIManager : MonoBehaviour
{
    [Header("UI��� : ���� �ð�")]
    [SerializeField]
    private Text text_Timer;

    [Header("UI��� : EXP & LEVEL")]
    [SerializeField]
    private Image image_ExpBar;
    [SerializeField]
    private Text text_Level;

    [Header("UI��� : �÷��̾� HP")]
    [SerializeField]
    private Image image_HPBar;

    [Header("UI��� : ���� HP")]
    [SerializeField]
    private Image image_BossHPBar;
    [SerializeField]
    private GameObject bossHPBarPanel;
    [SerializeField]
    private Text text_BossName;

    [Header("UI��� : ������ ������")]
    [SerializeField]
    private GameObject upgradePanel;
    [SerializeField]
    private GameObject altRewardPanel;
    [SerializeField]
    private SkillOptionUI[] skillOptions;
    [SerializeField]
    private Text upgradePanelTitle;
    [SerializeField]
    private Text upgradePanelDescription;
    private bool isSelecting = false;
    private int pendingSelection = 0;

    [Header("UI��� : ���� ��ų �ε�������")]
    [SerializeField]
    private SkillSlotUI[] skillSlots;

    [Header("UI��� : ���� ����")]
    [SerializeField]
    private GameObject weaponSelectionPanel;

    //���۷���
    private SkillManager _skillManager;
    private TimeScaleManager _scaleManager;
    private UIController _UIController;
    private Enemy _currentBoss;
    private WeaponManager _weaponManager;

    //UI Events
    public static event Action HPRewardSelected;
    public static event Action AMMORewardSelected;

    private void Awake()
    {
        _skillManager = FindFirstObjectByType<SkillManager>();
        _scaleManager = FindFirstObjectByType<TimeScaleManager>();
        _weaponManager = FindFirstObjectByType<WeaponManager>();
        _UIController = FindFirstObjectByType<UIController>();
        if(_weaponManager == null)
        {
            Debug.LogError("�����Ŵ��� �� ã��!");
        }
        if (_UIController == null)
        {
            Debug.LogError("UIController ã�� ����!");
        }
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        if (altRewardPanel != null)
        {
            altRewardPanel.SetActive(false);
        }
        if (upgradePanelTitle != null)
        {
            upgradePanelTitle.gameObject.SetActive(false);
        }
        if (bossHPBarPanel != null)
        {
            bossHPBarPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (weaponSelectionPanel != null)
            ShowWeaponSelectionUI();
    }
    private void Update()
    {
        HandleBossUI();
    }

    //------------ ������ ����Ŭ ----------------------
    void OnEnable()
    {
        EXPManager.OnExpChanged += UpdateExpBar;
        EXPManager.OnLevelChanged += UpdateLevel;
        HealthController.OnHealthChanged += UpdateHP;
        TimeManager.OnTimeChanged += UpdateTime;
        SkillManager.OnSkillsUpdated += UpdateSkillIndicator;
        WeaponSelectionManager.OnWeaponSelected += EquipInitialWeapon;
    }

    void OnDisable()
    {
        EXPManager.OnExpChanged -= UpdateExpBar;
        EXPManager.OnLevelChanged -= UpdateLevel;
        HealthController.OnHealthChanged -= UpdateHP;
        TimeManager.OnTimeChanged -= UpdateTime;
        SkillManager.OnSkillsUpdated -= UpdateSkillIndicator;
        WeaponSelectionManager.OnWeaponSelected -= EquipInitialWeapon;
    }

    //----------------- TIME ---------------------------
    private void UpdateTime(float elapsedTime)
    {
        if(text_Timer != null)
        {
            TimeSpan ts = TimeSpan.FromSeconds(elapsedTime);
            text_Timer.text = string.Format("{0:D2}:{1:D2}",
                ts.Minutes, ts.Seconds);
        }
    }
    //----------------- EXP & LEVEL ----------------------
    private void UpdateExpBar(float currentExp, float maxExp)
    {
        //����ġ �� ������Ʈ
        if (image_ExpBar != null)
            image_ExpBar.fillAmount = currentExp / maxExp;
    }
    private void UpdateLevel(int level)
    {
        if (text_Level != null)
            text_Level.text = "Lv : " + level.ToString();
    }

    //--------------------- HP -------------------------
    private void UpdateHP(float currentHP, float maxHp)
    {
        if (image_HPBar != null)
            image_HPBar.fillAmount = currentHP / maxHp;
    }

    //--------------------- BOSS HP -----------------------
    private void HandleBossUI()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        //���� ������ �ִ� ���
        if(boss != null)
        {
            Enemy bossEnemy = boss.GetComponent<Enemy>();

            if(_currentBoss != bossEnemy)
            {
                if (_currentBoss != null)
                    _currentBoss.OnHealthChanged -= UpdateBossHP;

                _currentBoss = bossEnemy;
                _currentBoss.OnHealthChanged += UpdateBossHP;
            }

            if (bossHPBarPanel != null)
                bossHPBarPanel.SetActive(true);

            if (text_BossName != null)
                text_BossName.text = _currentBoss.GetName();

            UpdateBossHP(_currentBoss.GetCurrentHealth(), _currentBoss.GetMaxHealth());
        }

        //���� ������ ���� ���
        else
        {
            if(bossHPBarPanel != null)
                bossHPBarPanel.SetActive(false);
            if(_currentBoss != null)
            {
                _currentBoss.OnHealthChanged -= UpdateBossHP;
                _currentBoss = null;
            }
        }
    }
    private void UpdateBossHP(float currentHealth, float maxHealth)
    {
        if(image_BossHPBar != null)
            image_BossHPBar.fillAmount = currentHealth / maxHealth;
    }

    //----------------------- Select Weapon ----------------------
    private void EquipInitialWeapon(Gun weaponPrefab)
    {
        if(_weaponManager != null)
            _weaponManager.EquipInitWeapon(weaponPrefab);

        if(weaponSelectionPanel != null)
            weaponSelectionPanel.SetActive(false);

        if(_UIController != null && _UIController.HUDCanvas != null)
            _UIController.HUDCanvas.SetActive(true);

        //Ȱ��
        _scaleManager.Resume();
        AudioListener.pause = false;
        _UIController.EnableInputBindings();
        HideCursor(true);
    }
    private void ShowWeaponSelectionUI()
    {
        weaponSelectionPanel.SetActive(true);

        if (_UIController != null && _UIController.HUDCanvas != null)
            _UIController.HUDCanvas.SetActive(false);
        //��Ȱ��
        _scaleManager.Pause();
        AudioListener.pause = true;
        _UIController.DisableInputBindings();
        HideCursor(false);
    }

    //------------------ ��ų ���׷��̵� ������ -----------
    public void ShowUpgradeUI(KeyValuePair<SkillData, int> skill1, KeyValuePair<SkillData, int> skill2, KeyValuePair<SkillData, int>skill3)
    {
        if (isSelecting)
        {
            pendingSelection++;
            return;
        }

        isSelecting = true;
        _UIController.UpgradeSelecting = true;

        //pause
        _scaleManager.Pause();
        AudioListener.pause = true;
        _UIController.DisableInputBindings();
        HideCursor(false);
        _UIController.HUDCanvas.SetActive(false);

        upgradePanelTitle.text = "���� ��!";
        upgradePanelDescription.text = "���� �� �������� ��ų�� ���׷��̵� �ϰų� ���� �����ϼ���!";
        upgradePanelTitle.gameObject.SetActive(true);
        upgradePanel.SetActive(true);

        KeyValuePair<SkillData, int>[] options = { skill1, skill2, skill3 };

        for (int i = 0; i < options.Length; i++)
        {
            skillOptions[i].skillSelectButton.onClick.RemoveAllListeners();
            ShowSkillOption(options, i);
        }
    }

    private void ShowSkillOption(KeyValuePair<SkillData, int>[] options, int i)
    {
        if (options[i].Key != null)
        {
            var currentOption = options[i];
            var skillData = currentOption.Key;
            int skillLevel = currentOption.Value;

            skillOptions[i].container.SetActive(true);
            skillOptions[i].skillNameText.text = skillData.skillName;
            skillOptions[i].skillIconImage.sprite = skillData.skillIcon;
            UpdateSkillDescription(i, skillData, skillLevel);

            skillOptions[i].skillSelectButton.onClick.AddListener(() => SelectSkillAndCloseUI(skillData));
            return;
        }
        skillOptions[i].container.SetActive(false);
    }

    private void UpdateSkillDescription(int i, SkillData skillData, int skillLevel)
    {
        if (skillLevel > 1)
        {
            skillOptions[i].skillDescriptionText.text = skillData.GetGenericLevelInfo(skillLevel).upgradeDescription;
        }
        else
        {
            skillOptions[i].skillDescriptionText.text = skillData.description;
        }
    }

    public void SelectSkillAndCloseUI(SkillData skillData)
    {
        _skillManager.LearnOrUpgradeSkill(skillData);
        isSelecting = false;
        ResumeWhenSelectingFinished();
    }

    public void SelectHPReward()
    {
        HPRewardSelected?.Invoke();
        isSelecting = false;
        ResumeWhenSelectingFinished();
    }

    public void SelectAMMOReward()
    {
        AMMORewardSelected?.Invoke();
        isSelecting = false;
        ResumeWhenSelectingFinished();
    }

    private void ResumeWhenSelectingFinished()
    {
        if (!IsSelectionPending())
        {
            //RESUME
            _UIController.HUDCanvas.SetActive(true);
            upgradePanel.SetActive(false);
            altRewardPanel.SetActive(false);
            upgradePanelTitle.gameObject.SetActive(false);
            _scaleManager.Resume();
            AudioListener.pause = false;
            HideCursor(true);
            _UIController.EnableInputBindings();
            _UIController.UpgradeSelecting = false;
        }
    }

    private bool IsSelectionPending()
    {
        if (pendingSelection > 0)
        {
            upgradePanel.SetActive(false);
            altRewardPanel.SetActive(false);
            _skillManager.LevelUp();
            pendingSelection--;
            return true;
        }
        return false;
    }

    public void ShowAltLevelupReward()
    {
        if (isSelecting)
        {
            pendingSelection++;
            return;
        }

        isSelecting = true;
        _UIController.UpgradeSelecting = true;

        //pause
        _scaleManager.Pause();
        AudioListener.pause = true;
        _UIController.DisableInputBindings();
        HideCursor(false);
        _UIController.HUDCanvas.SetActive(false);

        upgradePanelTitle.text = "��� ��ų �ִ� ���׷��̵�";
        upgradePanelDescription.text = "��� ��ų�� �ִ�� ���׷��̵� �Ͽ����ϴ�. ��ü ������ �����ϼ���.";
        upgradePanelTitle.gameObject.SetActive(true);
        altRewardPanel.SetActive(true);
    }



    //------------------ ���� ��ų �ε������� -----------
    private void UpdateSkillIndicator(SerializableDictionary<SkillData, int> ownedSkills)
    {
        int skillSlotIndex = 0;
        foreach(var skill in ownedSkills)
        {
            SkillData skillData = skill.Key;
            int skillLevel = skill.Value;

            if(skillSlotIndex >= skillSlots.Length)
            {
                Debug.LogWarning("���� ��ų �ε������� ������ �����մϴ�. �߰� ������ �����ϼ���.");
                break;
            }
            skillSlots[skillSlotIndex].container.SetActive(true);
            skillSlots[skillSlotIndex].skillIconImage.sprite = skillData.skillIcon;
            skillSlots[skillSlotIndex].skillLevelText.text = skillLevel.ToString();
            skillSlotIndex++;
        }
    }

    //---------------- ���� �޼��� ----------------
    private static void HideCursor(bool hide)
    {
        if (hide)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

[System.Serializable]
public class SkillOptionUI
{
    public Text skillNameText;
    public Text skillDescriptionText;
    public Image skillIconImage;
    public Button skillSelectButton;
    public GameObject container;
}

[Serializable]
public class SkillSlotUI
{
    public Image skillIconImage;
    public Text skillLevelText;
    public GameObject container;
}
