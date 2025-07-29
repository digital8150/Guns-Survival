using UnityEngine;
using UnityEngine.Pool;

public class EXP : MonoBehaviour
{
    [Header("Ä¸½¶ °æÇèÄ¡·®")]
    public float exp;

    private Renderer capsuleRenderer;       //Ä¸½¶ »ö
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

    //----------------------- ÇÃ·¹ÀÌ¾î - °æÇèÄ¡ Ä¸½¶ Á¢ÃË ------------------------
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EXPManager xPManager = other.GetComponent<EXPManager>();

            if(xPManager != null)
            {
                //°æÇèÄ¡ ½Àµæ
                xPManager.AddExp(exp);

                //°æÇèÄ¡ ¿ÉÁ§ ÆÄ±«(Ç®¸µ ÀÌ¿ë)
                if(m_Pool != null)
                    m_Pool.Release(gameObject);
                else
                    Destroy(gameObject);
            }
        }
    }

    //------------------------ Àû¸¶´Ù Ä¸½¶ »ö º¯°æ ---------------------
    public void SetColor(Color color)
    {
        if (capsuleRenderer != null)
            capsuleRenderer.material.color = color;
    }
}
