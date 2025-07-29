using UnityEngine;
using UnityEngine.Pool;

public class EXPPool : MonoBehaviour
{
    public GameObject exp;
    private ObjectPool<GameObject> m_Pool;

    private void Awake()
    {
        m_Pool = new ObjectPool<GameObject>
            (CreatePoolObj,
            OnTakeCapsule,
            OnReleaseCapsule,
            OnDestroyCapsule,
            true, 10, 100);
    }

    private GameObject CreatePoolObj()
    {
        GameObject capsule = Instantiate(exp);
        capsule.GetComponent<EXP>().SetPool(m_Pool);
        return capsule;
    }

    //풀에 접근하는 메소드 3개
    private void OnTakeCapsule(GameObject capsule)
    {
        capsule.SetActive(true);
        Debug.Log("오브젝트 꺼내기 완료");
    }

    private void OnReleaseCapsule(GameObject capsule)
    {
        capsule.SetActive(false);
        Debug.Log("오브젝트 반납 완료");

    }


    private void OnDestroyCapsule(GameObject capsule)
    {
        Destroy(capsule);
        Debug.Log("오브젝트 제거 완료");
    }

    //참조용
    public GameObject Get() => m_Pool.Get();
}
