using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent를 사용하기 위해 필요

public class EnemyAI : MonoBehaviour
{
    [Header("적 AI 기본 설정")]
    [SerializeField] float detectionRange = 10f; // 플레이어를 감지할 거리
    [SerializeField] float stoppingDistance = 1.5f; // 플레이어에게 얼마나 가까이 다가갈지
    [SerializeField] float attackRange = 2f; // 공격을 시작할 거리 (선택 사항)
    [SerializeField] float attackCooldown = 2f; // 공격 쿨타임 (선택 사항)
    [SerializeField] float damage = 10;

    //컴포넌트 연결
    Enemy enemy;
    Transform playerTransform; // 플레이어의 Transform
    DamageHandler damageHandler;
    

    private NavMeshAgent agent;
    private bool isAttacking = false;
    private float lastAttackTime;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        playerTransform = FindFirstObjectByType<FirstPersonCharacterController>().transform;
        damageHandler = playerTransform.gameObject.GetComponent<DamageHandler>();
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent 컴포넌트가 없습니다! 적 오브젝트에 추가해주세요.");
        }
        // NavMeshAgent의 정지 거리를 설정하여 플레이어에게 너무 가까이 가지 않도록 함
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (!enemy.IsAlive)
        {
            agent.isStopped = true;
            return;
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("플레이어 Transform이 할당되지 않았습니다. 인스펙터에서 할당해주세요.");
            return;
        }

        // 플레이어와 적 사이의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 플레이어가 감지 범위 내에 있을 경우
        if (distanceToPlayer <= detectionRange)
        {
            // 플레이어를 향해 이동
            agent.SetDestination(playerTransform.position);

            // 플레이어에게 충분히 가까워졌을 때 (멈춤 거리 이내)
            if (distanceToPlayer <= stoppingDistance)
            {
                // 이동 멈춤
                agent.isStopped = true;

                // 플레이어를 바라보게 함
                LookAtPlayer();

                // 공격 범위 내에 있고, 쿨타임이 지났으면 공격
                if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else // 플레이어에게 아직 도달하지 못했으면 계속 이동
            {
                agent.isStopped = false;
            }
        }
        else // 플레이어가 감지 범위 밖에 있을 경우
        {
            // 이동 멈춤
            agent.isStopped = true;
        }
    }

    void LookAtPlayer()
    {
        // 플레이어를 바라보는 방향 벡터 계산 (Y축 고정)
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // 부드러운 회전
    }

    void AttackPlayer()
    {
        // 여기에 적의 공격 로직을 구현합니다.
        // 예: 애니메이션 재생, 데미지 주기 등
        enemy.AnimSetTrigger("Attack");
        lastAttackTime = Time.time; // 마지막 공격 시간 갱신
        damageHandler.Damage(damage);
        // isAttacking = true; // 애니메이션 등을 사용하는 경우 설정
    }

    // 디버깅을 위해 감지 범위와 공격 범위를 기즈모로 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}