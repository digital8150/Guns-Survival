using System;
using UnityEngine;

//시간을 측정하는 스크립트
//시간을 멈추는 스크립트는 TimeScaleManager에 의존
public class TimeManager : MonoBehaviour
{
    public float ElapsedTime {  get; private set; }

    public static event Action<float> OnTimeChanged;


    void Start()
    {
        ElapsedTime = 0f;
    }

    void Update()
    {
        ElapsedTime += Time.deltaTime;
        OnTimeChanged?.Invoke(ElapsedTime);
    }
}
