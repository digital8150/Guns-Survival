using UnityEngine;
using GameBuilders.FPSBuilder;
using GameBuilders.FPSBuilder.Interfaces;
using System.Runtime.CompilerServices;
using System;

public class Enemy : MonoBehaviour, IProjectileDamageable
{
    [Header("적 기본 정보")]
    [SerializeField]
    private string enemyName;
    [SerializeField]
    private float hp;
    [SerializeField]
    private float currentHp;
    [SerializeField]
    private float destroyDelay;
    [SerializeField]
    private GameObject expPrefab;                   //캡슐 프리팹
    [SerializeField]
    private float exp;                              //적이 드랍한 처치 경험치
    [SerializeField]
    private Color capsuleColor = Color.blue;        //경험치 캡슐 기본 색
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

    public event Action<float, float> OnHealthChanged;      //보스 체력 이벤트

    void Start()
    {
        currentHp = hp;
        IsAlive = true;
        deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");
        expPool = FindAnyObjectByType<EXPPool>();
        //일반 몹도 이벤트에 걸리기 때문에 isBoss flag 세워서 단독 이벤트로 변경
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
        //EXP 생성
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
            Debug.Log("EXPPool을 찾을 수 없으므로 직접 경험치 캡슐을 생성합니다");
            Instantiate(expPrefab, transform.position, Quaternion.identity);
        }
    }

    //UI 반환용
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
