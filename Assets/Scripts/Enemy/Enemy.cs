using UnityEngine;
using GameBuilders.FPSBuilder;
using GameBuilders.FPSBuilder.Interfaces;
using System.Runtime.CompilerServices;

public class Enemy : MonoBehaviour, IProjectileDamageable
{
    [Header("적 기본 정보")]
    [SerializeField]
    private float hp;
    [SerializeField]
    private float currentHp;
    [SerializeField]
    private float destroyDelay;
    [SerializeField]
    private GameObject expPrefab;       //캡슐 프리팹
    [SerializeField]
    private float exp;            //적이 드랍한 처치 경험치
    [SerializeField]
    private Color capsuleColor = Color.blue;        //경험치 캡슐 기본 색

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

    void Start()
    {
        currentHp = hp;
        IsAlive = true;
        deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");
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

        hp -= damage;
        if(hp <= 0)
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

        CreateExp();

        Destroy(this.gameObject, destroyDelay);
    }

    void CreateExp()
    {
        //EXP 생성
        if (expPrefab != null)
        {
            GameObject capsule = Instantiate(expPrefab, transform.position, Quaternion.identity);
            EXP exp = capsule.GetComponent<EXP>();

            if (exp != null)
            {
                exp.exp = this.exp;
                exp.SetColor(this.capsuleColor);
            }
        }
    }
}
