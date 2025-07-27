using UnityEngine;
using System.Collections.Generic;

// 이 스크립트는 Sphere 오브젝트에 부착됩니다.
public class EXPAttractionController : MonoBehaviour
{
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

            foreach (GameObject capsuleGO in expCapsulesInScene)
            {
                // 각 캡슐에서 EXP 스크립트 가져오기
                EXP expScript = capsuleGO.GetComponent<EXP>();

                if (expScript != null)
                {
                    // 플레이어에게 경험치 추가
                    playerExpManager.AddExp(expScript.exp);

                    // 경험치 캡슐 파괴
                    Destroy(capsuleGO);
                }
            }
            Destroy(gameObject); // Sphere 오브젝트 자체를 파괴
        }
    }
}