using UnityEngine;
using UnityEngine.Pool;

public class EXP : MonoBehaviour
{
    [Header("캡슐 경험치량")]
    public float exp;

    private Renderer capsuleRenderer;       //캡슐 색
    private Rigidbody rb;
    private Movement3D movement3D;

    private IObjectPool<GameObject> m_Pool;

    private void Awake()
    {
        capsuleRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        movement3D = GetComponent<Movement3D>();
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        m_Pool = pool;
    }

    //----------------------- 플레이어 - 경험치 캡슐 접촉 ------------------------
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EXPManager xPManager = other.GetComponent<EXPManager>();

            if(xPManager != null)
            {
                //경험치 습득
                xPManager.AddExp(exp);

                //경험치 옵젝 파괴(풀링 이용)
                if (m_Pool != null)
                {
                    m_Pool.Release(gameObject);
                }
                else
                    Destroy(gameObject);
            }
        }
    }

    //------------------------ 적마다 캡슐 색 변경 ---------------------
    public void SetColor(Color color)
    {
        if (capsuleRenderer != null)
            capsuleRenderer.material.color = color;
    }

    private void OnDisable()
    {
        // 풀에 반환될 때 이전에 변경했던 상태들을 기본값으로 되돌립니다.
        if (rb != null) rb.useGravity = true; 
        if (movement3D != null) movement3D.MoveSpeed = 0;
        gameObject.layer = LayerMask.NameToLayer("Default"); 
    }
}
