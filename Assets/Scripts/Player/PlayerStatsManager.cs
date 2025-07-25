using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    private HealthController healthController; //�ִ�ü�� ���� ������
    private EXPManager expManager; //����ġ ȹ�淮 ���� ������
    private FirstPersonCharacterController firstPersonCharacterController; //�̵� �ӵ� ������

    //---movement feature
    private float originMovementSpeed;

    /// <summary>
    /// ������Ʈ ����
    /// </summary>
    private void Start()
    {
        healthController = GetComponent<HealthController>();
        expManager = GetComponent<EXPManager>();
        firstPersonCharacterController = GetComponent<FirstPersonCharacterController>();
        originMovementSpeed = firstPersonCharacterController.WalkingForce;
    }

    /// <summary>
    /// �÷��̾� �ִ� ü�� ����
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseMaxHp(float amount)
    {
        healthController.IncreaseTotalVitality(amount);
    }

    /// <summary>
    /// ����ġ ȹ�� ���� ����
    /// </summary>
    /// <param name="multAmount"></param>
    public void SetExpGainMult(float multAmount)
    {
        expManager.SetExpGainMult(multAmount);
    }

    /// <summary>
    /// �̵��ӵ� ���� ����
    /// </summary>
    /// <param name="multAmount"></param>
    public void IncreaseMovementSpeed(float multAmount)
    {
        firstPersonCharacterController.WalkingForce = originMovementSpeed * multAmount; //���� ���Ͽ� ����
    }

    /// TODO : ������ �ӵ� ���� -> ������ �ӵ��� ��� �����ϴ��� �߰� �ľ� �ʿ�


}
