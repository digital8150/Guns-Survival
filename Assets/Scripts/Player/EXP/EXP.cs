using UnityEngine;

public class EXP : MonoBehaviour
{
    [Header("Ä¸½¶ °æÇèÄ¡·®")]
    public float exp;

    private Renderer capsuleRenderer;       //Ä¸½¶ »ö

    private void Awake()
    {
        capsuleRenderer = GetComponent<Renderer>();
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

                //°æÇèÄ¡ ¿ÉÁ§ ÆÄ±«
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
