using System.Collections.Generic;
using System.Threading.Tasks;
using GameBuilders.MinimalistUI.Scripts;
using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI��� : EXP & LEVEL")]
    [SerializeField]
    private Image image_ExpBar;
    [SerializeField]
    private Text text_Level;

    [Header("UI��� : HP")]
    [SerializeField]
    private Image image_HPBar;

    [Header("UI��� : ������ ������")]
    [SerializeField]
    private GameObject upgradePanel;
    [SerializeField]
    private SkillOptionUI[] skillOptions;
    private bool isSelecting = false;
    private int pendingSelection = 0;

    //���۷���
    private SkillManager _skillManager;
    private TimeScaleManager _scaleManager;
    private UIController _UIController;

    private void Awake()
    {
        _skillManager = FindFirstObjectByType<SkillManager>();
        _scaleManager = FindFirstObjectByType<TimeScaleManager>();
        _UIController = GetComponent<UIController>();
        if(upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    //------------ ������ ����Ŭ ----------------------
    void OnEnable()
    {
        EXPManager.OnExpChanged += UpdateExpBar;
        EXPManager.OnLevelChanged += UpdateLevel;
        HealthController.OnHealthChanged += UpdateHP;
    }

    void OnDisable()
    {
        EXPManager.OnExpChanged -= UpdateExpBar;
        EXPManager.OnLevelChanged -= UpdateLevel;
        HealthController.OnHealthChanged -= UpdateHP;
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
            text_Level.text = "Level : " + level.ToString();
    }

    //--------------------- HP -------------------------
    private void UpdateHP(float  currentHP, float maxHp)
    {
        if(image_HPBar != null)
            image_HPBar.fillAmount = currentHP / maxHp;
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
            Debug.Log($"��ų �ɼ� {i}�� ��ư�� ������ ��� : {skillData.skillName}");
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
        Debug.Log($"��ư Ŭ�� : {skillData.skillName}");
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
