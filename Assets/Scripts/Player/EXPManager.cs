using UnityEngine;

public class EXPManager : MonoBehaviour
{
    [Header("플레이어 경험치 및 레벨 관리")]
    [SerializeField]
    private int level = 1;              //현재 레벨
    [SerializeField]
    private float currentExp = 0;       //실시간 확인용으로 직렬화시킴
    [SerializeField]
    private float maxExp = 100;         //최대 경험치
    [SerializeField]
    private float nextLevelPenalty = 1.0f;     //다음 경험치의 최대량 비율
    private float expGainMult = 1.0f; // 경험치 획득 배율
    private SkillManager skillManager; //스킬매니저 컴포넌트

    private void Start()
    {
        skillManager = GetComponent<SkillManager>();
    }

    //---------------경험치 획득 배율 설정
    public void SetExpGainMult(float expGainMult)
    {
        this.expGainMult = expGainMult;
    }

    //---------------경험치가 maxExp를 넘으면 레벨업시킴----------------
    public void AddExp(float exp)
    {
        currentExp += exp * expGainMult; //경험치 획득 배율 반영
        Debug.Log($"경험치 {exp} * {expGainMult} = {exp * expGainMult} 만큼 획득!");

        if (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    //---------------------------- 레벨업 ---------------------------
    public void LevelUp()
    {
        level++;                    //레벨 + 1
        currentExp -= maxExp;       //잔여 경험치 이동
        maxExp *= nextLevelPenalty; //다음 레벨에 필요한 경험치를 해당 비율만큼 증가
        
        skillManager.LevelUp();//스킬매니저 컴포넌트의 레벨업 호출

        Debug.Log($"레벨 업! 현재 레벨 {level}");
    }
}
