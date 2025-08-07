using System.Collections.Generic;
using GameBuilders.MinimalistUI.Scripts;
using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Drawing.Printing;
using static UnityEngine.InputSystem.XR.TrackedPoseDriver;

public class UIManager : MonoBehaviour
{
    [Header("UI요소 : 게임 시간")]
    [SerializeField]
    private Text text_Timer;

    [Header("UI요소 : EXP & LEVEL")]
    [SerializeField]
    private Image image_ExpBar;
    [SerializeField]
    private Text text_Level;

    [Header("UI요소 : 플레이어 HP")]
    [SerializeField]
    private Image image_HPBar;

    [Header("UI요소 : 보스 HP")]
    [SerializeField]
    private Image image_BossHPBar;
    [SerializeField]
    private GameObject bossHPBarPanel;
    [SerializeField]
    private Text text_BossName;

    [Header("UI요소 : 레벨업 선택지")]
    [SerializeField]
    private GameObject upgradePanel;
    [SerializeField]
    private SkillOptionUI[] skillOptions;
    private bool isSelecting = false;
    private int pendingSelection = 0;

    //레퍼런스
    private SkillManager _skillManager;
    private TimeScaleManager _scaleManager;
    private UIController _UIController;
    private Enemy _currentBoss;

    private void Awake()
    {
        _skillManager = FindFirstObjectByType<SkillManager>();
        _scaleManager = FindFirstObjectByType<TimeScaleManager>();
        _UIController = GetComponent<UIController>();
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        if (bossHPBarPanel != null)
        {
            bossHPBarPanel.SetActive(false);
        }
    }

    private void Update()
    {
        HandleBossUI();
    }

    //------------ 라이프 사이클 ----------------------
    void OnEnable()
    {
        EXPManager.OnExpChanged += UpdateExpBar;
        EXPManager.OnLevelChanged += UpdateLevel;
        HealthController.OnHealthChanged += UpdateHP;
        TimeManager.OnTimeChanged += UpdateTime;
    }

    void OnDisable()
    {
        EXPManager.OnExpChanged -= UpdateExpBar;
        EXPManager.OnLevelChanged -= UpdateLevel;
        HealthController.OnHealthChanged -= UpdateHP;
        TimeManager.OnTimeChanged -= UpdateTime;
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
        //경험치 바 업데이트
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

        //씬에 보스가 있는 경우
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

        //씬에 보스가 없는 경우
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

    //------------------ 스킬 업그레이드 선택지 -----------
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
        _UIController.HUDCanvas.SetActive(false);
        _scaleManager.Pause();
        upgradePanel.SetActive(true);
        AudioListener.pause = true;
        _UIController.DisableInputBindings();
        HideCursor(false);

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
            Debug.Log($"스킬 옵션 {i}번 버튼에 리스너 등록 : {skillData.skillName}");
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
        Debug.Log($"버튼 클릭 : {skillData.skillName}");
        _skillManager.LearnOrUpgradeSkill(skillData);
        isSelecting = false;

        if(pendingSelection > 0)
        {
            _skillManager.LevelUp();
            pendingSelection--;
        }
        else
        {
            //RESUME
            _UIController.HUDCanvas.SetActive(true);
            upgradePanel.SetActive(false);
            _scaleManager.Resume();
            AudioListener.pause = false;
            HideCursor(true);
            _UIController.EnableInputBindings();
            _UIController.UpgradeSelecting = false;
        }

    }

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
