using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    private HealthController healthController; //최대체력 스텟 의존성
    private EXPManager expManager; //경험치 획득량 배율 의존성
    private FirstPersonCharacterController firstPersonCharacterController; //이동 속도 의존성

    //---movement feature
    private float originMovementSpeed;

    /// <summary>
    /// 컴포넌트 연결
    /// </summary>
    private void Start()
    {
        healthController = GetComponent<HealthController>();
        expManager = GetComponent<EXPManager>();
        firstPersonCharacterController = GetComponent<FirstPersonCharacterController>();
        originMovementSpeed = firstPersonCharacterController.WalkingSpeed;
    }

    /// <summary>
    /// 플레이어 최대 체력 증가
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseMaxHp(float amount)
    {
        healthController.IncreaseTotalVitality(amount);
    }

    /// <summary>
    /// 경험치 획득 배율 설정
    /// </summary>
    /// <param name="multAmount"></param>
    public void SetExpGainMult(float multAmount)
    {
        expManager.SetExpGainMult(multAmount);
    }

    /// <summary>
    /// 이동속도 배율 설정
    /// </summary>
    /// <param name="multAmount"></param>
    public void IncreaseMovementSpeed(float multAmount)
    {
        firstPersonCharacterController.WalkingSpeed = originMovementSpeed * multAmount; //배율 곱하여 적용
    }

    /// TODO : 재장전 속도 증가 -> 재장전 속도를 어디서 관장하는지 추가 파악 필요


}
