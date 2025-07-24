using UnityEngine;

public class EXP : MonoBehaviour
{
    [Header("ĸ�� ����ġ��")]
    [SerializeField]
    private float exp = 25.0f;


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
}
