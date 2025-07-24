using UnityEngine;

public class EXP : MonoBehaviour
{
    [Header("ĸ�� ����ġ��")]
    public float exp;

    private Renderer capsuleRenderer;       //ĸ�� ��

    private void Awake()
    {
        capsuleRenderer = GetComponent<Renderer>();
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

                //����ġ ���� �ı�
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
