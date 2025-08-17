using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
public class Hitmarker : MonoBehaviour
{

    [SerializeField]
    private GameObject m_Hitmarker;

    [SerializeField]
    private float m_Duration = 0.25f;

    private readonly List<GameObject> m_IndicatorList = new List<GameObject>();

    //---- 라이프 사이클 ----
    private void OnEnable()
    {
        Enemy.OnHit += ShowHitmarker;
    }

    private void OnDisable()
    {
        Enemy.OnHit -= ShowHitmarker;
    }


    private void ShowHitmarker()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHitmarkerCoroutine());
    }

    private IEnumerator ShowHitmarkerCoroutine()
    {
        SetHitmarkerActive(true);
        yield return new WaitForSeconds(m_Duration);
        SetHitmarkerActive(false);
    }

    private void SetHitmarkerActive(bool flag)
    {
        if (m_Hitmarker != null)
        {
            m_Hitmarker.SetActive(flag);
        }
    }
}
