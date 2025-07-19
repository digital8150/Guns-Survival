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

    void Start()
    {
        currentHp = hp;
        IsAlive = true;
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

    void AnimSetTrigger(string name)
    {
        animator1.SetTrigger(name);
        animator2.SetTrigger(name);
    }
}
