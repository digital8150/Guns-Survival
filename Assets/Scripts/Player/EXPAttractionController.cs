using UnityEngine;
using System.Collections.Generic;
using GameBuilders.FPSBuilder.Core.Player;

// 이 스크립트는 Sphere 오브젝트에 부착됩니다.
public class EXPAttractionController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("경허치 구슬들이 플레이어로 딸려오는 속도")]
    private float moveSpeed = 3.0f;

    //----------------------- Sphere - 플레이어 접촉 ------------------------
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어의 EXPManager 컴포넌트 가져오기 (모든 경험치 획득에 사용)
            EXPManager playerExpManager = other.GetComponent<EXPManager>();

            // 씬에 있는 모든 "EXP" 태그를 가진 GameObject 찾기
            GameObject[] expCapsulesInScene = GameObject.FindGameObjectsWithTag("EXP");
            var player = FindFirstObjectByType<FirstPersonCharacterController>().transform;
            foreach (GameObject capsuleGO in expCapsulesInScene)
            {
                //충돌 끄기
                capsuleGO.layer = LayerMask.NameToLayer("Noclip");
                //중력 off
                capsuleGO.GetComponent<Rigidbody>().useGravity = false;
                //플레이어를 향해 이동
                capsuleGO.AddComponent<Movement3D>().MoveSpeed = moveSpeed;
                var moveTo = capsuleGO.AddComponent<MoveTo>();
                moveTo.Setup(player);
            }
            Destroy(gameObject); // Sphere 오브젝트 자체를 파괴
        }
    }
}