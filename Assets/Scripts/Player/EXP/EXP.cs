using UnityEngine;

public class EXP : MonoBehaviour
{
    [Header("캡슐 경험치량")]
    [SerializeField]
    private float exp = 25.0f;


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

                //경험치 옵젝 파괴
                Destroy(gameObject);
            }
        }
    }
}
