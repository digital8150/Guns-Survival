using System;
using GameBuilders.FPSBuilder.Core.Inventory;
using GameBuilders.FPSBuilder.Core.Player;
using GameBuilders.FPSBuilder.Core.Weapons;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    private HealthController healthController; //�ִ�ü�� ���� ������
    private EXPManager expManager; //����ġ ȹ�淮 ���� ������
    private FirstPersonCharacterController firstPersonCharacterController; //�̵� �ӵ� ������
    [SerializeField] private WeaponManager weaponManager; //���� �޴��� ������
    [SerializeField] private Gun[] guns; //������ �ӵ� ������

    //---movement feature
    private float originMovementSpeed;

    //------ ������ ����Ŭ -----
    private void Start()
    {
        healthController = GetComponent<HealthController>();
        expManager = GetComponent<EXPManager>();
        firstPersonCharacterController = GetComponent<FirstPersonCharacterController>();
        originMovementSpeed = firstPersonCharacterController.WalkingSpeed;
    }

    private void OnEnable()
    {
        UIManager.HPRewardSelected += HPReward;
        UIManager.AMMORewardSelected += AMMOReward;
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
        firstPersonCharacterController.WalkingSpeed = originMovementSpeed * multAmount; //���� ���Ͽ� ����
    }

    /// TODO : ������ �ӵ� ���� -> ������ �ӵ��� ��� �����ϴ��� �߰� �ľ� �ʿ�
    public void SetReloadAnimationSpeed(float speed)
    {
        foreach(var gun in guns)
        {
            try
            {
                if(gun == null)
                {
                    return;
                }
                gun.SetRelodSpeed(speed);
            }catch(Exception ex)
            {
                Debug.LogError($"������ �ӵ� ���� �� ���� �߻� : {ex}");
            }
        }

    }

    void HPReward()
    {
        weaponManager.MedkitReward();
    }

    void AMMOReward()
    {
        weaponManager.RefillAmmoReward();
    }


}
