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

    //components
    [SerializeField]
    private Animator animator1;
    [SerializeField]
    private Animator animator2;

    //player position
    private Transform player;

    //private members
    private Vector3 previousPosition;

    void Start()
    {
        currentHp = hp;
        IsAlive = true;
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
            IsAlive = false;
            AnimSetTrigger("Dead");
            Debug.Log("적이 뒤졌습니다!");
            Destroy(this.gameObject, destroyDelay);
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
}
