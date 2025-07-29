using UnityEngine;
using UnityEngine.Pool;

public class EXP : MonoBehaviour
{
    [Header("ĸ�� ����ġ��")]
    public float exp;

    private Renderer capsuleRenderer;       //ĸ�� ��
    private Rigidbody rb;

    private IObjectPool<GameObject> m_Pool;

    private void Awake()
    {
        capsuleRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        m_Pool = pool;
    }

    //----------------------- �÷��̾� - ����ġ ĸ�� ���� ------------------------
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EXPManager xPManager = other.GetComponent<EXPManager>();

            if(xPManager != null)
            {
                //����ġ ����
                xPManager.AddExp(exp);

                //����ġ ���� �ı�(Ǯ�� �̿�)
                if(m_Pool != null)
                    m_Pool.Release(gameObject);
                else
                    Destroy(gameObject);
            }
        }
    }

    //------------------------ ������ ĸ�� �� ���� ---------------------
    public void SetColor(Color color)
    {
        if (capsuleRenderer != null)
            capsuleRenderer.material.color = color;
    }
}
