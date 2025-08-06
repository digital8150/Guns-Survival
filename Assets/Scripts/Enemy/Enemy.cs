using UnityEngine;
using GameBuilders.FPSBuilder;
using GameBuilders.FPSBuilder.Interfaces;
using System.Runtime.CompilerServices;
using System;

public class Enemy : MonoBehaviour, IProjectileDamageable
{
    [Header("�� �⺻ ����")]
    [SerializeField]
    private string enemyName;
    [SerializeField]
    private float hp;
    [SerializeField]
    private float currentHp;
    [SerializeField]
    private float destroyDelay;
    [SerializeField]
    private GameObject expPrefab;                   //ĸ�� ������
    [SerializeField]
    private float exp;                              //���� ����� óġ ����ġ
    [SerializeField]
    private Color capsuleColor = Color.blue;        //����ġ ĸ�� �⺻ ��
    [SerializeField]
    private bool isBoss = false;

    //components
    [SerializeField]
    private Animator animator1;
    [SerializeField]
    private Animator animator2;

    //player position
    private Transform player;

    //private members
    private Vector3 previousPosition;
    private int deadEnemyLayer;
    private EXPPool expPool;

    public event Action<float, float> OnHealthChanged;      //���� ü�� �̺�Ʈ

    void Start()
    {
        currentHp = hp;
        IsAlive = true;
        deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");
        expPool = FindAnyObjectByType<EXPPool>();
        //�Ϲ� ���� �̺�Ʈ�� �ɸ��� ������ isBoss flag ������ �ܵ� �̺�Ʈ�� ����
        if (isBoss)
            OnHealthChanged?.Invoke(currentHp, hp);
    }

    void FixedUpdate()
    {
        if ((transform.position - previousPosition).magnitude > 0.02f)
        {
            AnimSetBool("Moving", true);
        }
        else
        {
            AnimSetBool("Moving", false);
        }

        previousPosition = transform.position;
    }

    public bool IsAlive { get; private set; }

    public void Damage(float damage)
    {
        if (!IsAlive)
        {
            return;
        }

        currentHp = Mathf.Max(currentHp - damage, 0);

        if (isBoss)
            OnHealthChanged?.Invoke(currentHp, hp);

        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void Damage(float damage, Vector3 targetPosition, Vector3 hitPosition)
    {
        Damage(damage);
    }

    public void ProjectileDamage(float damage, Vector3 targetPosition, Vector3 hitPosition, float penetrationPower)
    {
        Damage(damage);
    }

    public void AnimSetTrigger(string name)
    {
        if (animator1 != null) animator1.SetTrigger(name);
        if(animator2 != null) animator2.SetTrigger(name);
    }

    public void AnimSetBool(string name, bool value)
    {
        if(animator1 != null) animator1.SetBool(name, value);
        if(animator2 != null) animator2.SetBool(name, value);
    }

    void Die()
    {
        IsAlive = false;
        AnimSetTrigger("Dead");
        gameObject.layer = deadEnemyLayer;

        if (isBoss)
            OnHealthChanged?.Invoke(0, hp);

        CreateExp();
        Destroy(this.gameObject, destroyDelay);
    }

    void CreateExp()
    {
        //EXP ����
        if (expPrefab != null)
        {
            GameObject capsule = expPool.Get();
            capsule.transform.position = transform.position;
            capsule.transform.rotation = Quaternion.identity;

            EXP exp = capsule.GetComponent<EXP>();
            if (exp != null)
            {
                exp.exp = this.exp;
                exp.SetColor(this.capsuleColor);
            }
        }
        else
        {
            Debug.Log("EXPPool�� ã�� �� �����Ƿ� ���� ����ġ ĸ���� �����մϴ�");
            Instantiate(expPrefab, transform.position, Quaternion.identity);
        }
    }

    //UI ��ȯ��
    public float GetCurrentHealth()
    {
        return currentHp;
    }
    public float GetMaxHealth()
    {
        return hp;
    }
    public bool IsBoss()
    {
        return isBoss;
    }
    public string GetName()
    {
        return enemyName;
    }
}
