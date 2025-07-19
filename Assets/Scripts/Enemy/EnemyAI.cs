using GameBuilders.FPSBuilder.Core.Player;
using UnityEngine;
using UnityEngine.AI; // NavMeshAgent�� ����ϱ� ���� �ʿ�

public class EnemyAI : MonoBehaviour
{
    [Header("�� AI �⺻ ����")]
    [SerializeField] float detectionRange = 10f; // �÷��̾ ������ �Ÿ�
    [SerializeField] float stoppingDistance = 1.5f; // �÷��̾�� �󸶳� ������ �ٰ�����
    [SerializeField] float attackRange = 2f; // ������ ������ �Ÿ� (���� ����)
    [SerializeField] float attackCooldown = 2f; // ���� ��Ÿ�� (���� ����)
    [SerializeField] float damage = 10;

    //������Ʈ ����
    Enemy enemy;
    Transform playerTransform; // �÷��̾��� Transform
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
            Debug.LogError("NavMeshAgent ������Ʈ�� �����ϴ�! �� ������Ʈ�� �߰����ּ���.");
        }
        // NavMeshAgent�� ���� �Ÿ��� �����Ͽ� �÷��̾�� �ʹ� ������ ���� �ʵ��� ��
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
            Debug.LogWarning("�÷��̾� Transform�� �Ҵ���� �ʾҽ��ϴ�. �ν����Ϳ��� �Ҵ����ּ���.");
            return;
        }

        // �÷��̾�� �� ������ �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // �÷��̾ ���� ���� ���� ���� ���
        if (distanceToPlayer <= detectionRange)
        {
            // �÷��̾ ���� �̵�
            agent.SetDestination(playerTransform.position);

            // �÷��̾�� ����� ��������� �� (���� �Ÿ� �̳�)
            if (distanceToPlayer <= stoppingDistance)
            {
                // �̵� ����
                agent.isStopped = true;

                // �÷��̾ �ٶ󺸰� ��
                LookAtPlayer();

                // ���� ���� ���� �ְ�, ��Ÿ���� �������� ����
                if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else // �÷��̾�� ���� �������� �������� ��� �̵�
            {
                agent.isStopped = false;
            }
        }
        else // �÷��̾ ���� ���� �ۿ� ���� ���
        {
            // �̵� ����
            agent.isStopped = true;
        }
    }

    void LookAtPlayer()
    {
        // �÷��̾ �ٶ󺸴� ���� ���� ��� (Y�� ����)
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // �ε巯�� ȸ��
    }

    void AttackPlayer()
    {
        // ���⿡ ���� ���� ������ �����մϴ�.
        // ��: �ִϸ��̼� ���, ������ �ֱ� ��
        Debug.Log("�÷��̾ �����մϴ�!");
        enemy.AnimSetTrigger("Attack");
        lastAttackTime = Time.time; // ������ ���� �ð� ����
        damageHandler.Damage(damage);
        // isAttacking = true; // �ִϸ��̼� ���� ����ϴ� ��� ����
    }

    // ������� ���� ���� ������ ���� ������ ������ ǥ��
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}