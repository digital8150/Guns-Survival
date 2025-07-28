using UnityEngine;

public class MoveTo : MonoBehaviour
{
    private Movement3D movement3D;
    private Transform target;


    /// <summary>
    /// � ����� �߰��Ͽ� �̵��� �� ���� �¾�
    /// </summary>
    public void Setup(Transform target)
    {
        movement3D = GetComponent<Movement3D>();
        this.target = target;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            movement3D.MoveTo(direction);
        }
    }
}
