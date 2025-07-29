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

    //Ǯ�� �����ϴ� �޼ҵ� 3��
    private void OnTakeCapsule(GameObject capsule)
    {
        capsule.SetActive(true);
        Debug.Log("������Ʈ ������ �Ϸ�");
    }

    private void OnReleaseCapsule(GameObject capsule)
    {
        capsule.SetActive(false);
        Debug.Log("������Ʈ �ݳ� �Ϸ�");

    }


    private void OnDestroyCapsule(GameObject capsule)
    {
        Destroy(capsule);
        Debug.Log("������Ʈ ���� �Ϸ�");
    }

    //������
    public GameObject Get() => m_Pool.Get();
}
