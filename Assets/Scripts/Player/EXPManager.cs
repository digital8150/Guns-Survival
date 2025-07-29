using UnityEngine;
using UnityEngine.UI;

public class EXPManager : MonoBehaviour
{
    [Header("�÷��̾� ����ġ �� ���� ����")]
    [SerializeField]
    private int level = 1;              //���� ����
    [SerializeField]
    private float currentExp = 0;       //�ǽð� Ȯ�ο����� ����ȭ��Ŵ
    [SerializeField]
    private float maxExp = 100;         //�ִ� ����ġ
    [SerializeField]
    private float nextLevelPenalty = 1.0f;     //���� ����ġ�� �ִ뷮 ����
    private float expGainMult = 1.0f; // ����ġ ȹ�� ����
    private SkillManager skillManager; //��ų�Ŵ��� ������Ʈ

    [Header("UI ����")]
    [SerializeField]
    private Image image_ExpBar;
    [SerializeField]
    private Text text_Level;

    private void Start()
    {
        skillManager = GetComponent<SkillManager>();
        UpdateExpBar();
        UpdateLevel();
    }

    //---------------����ġ ȹ�� ���� ����
    public void SetExpGainMult(float expGainMult)
    {
        this.expGainMult = expGainMult;
    }

    //---------------����ġ�� maxExp�� ������ ��������Ŵ----------------
    public void AddExp(float exp)
    {
        currentExp += exp * expGainMult; //����ġ ȹ�� ���� �ݿ�
        Debug.Log($"����ġ {exp} * {expGainMult} = {exp * expGainMult} ��ŭ ȹ��!");

        //������ �ѹ��� ������ ��Ȳ�� ����� ������ while�� ����
        while (currentExp >= maxExp)
        {
            LevelUp();
        }

        UpdateExpBar();
    }

    //---------------------------- ������ ---------------------------
    public void LevelUp()
    {
        level++;                    //���� + 1
        currentExp -= maxExp;       //�ܿ� ����ġ �̵�
        maxExp *= nextLevelPenalty; //���� ������ �ʿ��� ����ġ�� �ش� ������ŭ ����
        
        skillManager.LevelUp();//��ų�Ŵ��� ������Ʈ�� ������ ȣ��

        UpdateLevel();
        UpdateExpBar();
    }

    //---------------------------- UI ------------------------
    private void UpdateExpBar()
    {
        //����ġ �� ������Ʈ
        if(image_ExpBar != null)
            image_ExpBar.fillAmount = currentExp / maxExp;
    }
    private void UpdateLevel()
    {
        if(text_Level != null)
            text_Level.text = "Level : " + level.ToString();
    }
}
