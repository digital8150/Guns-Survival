using System;
using UnityEngine;

//�ð��� �����ϴ� ��ũ��Ʈ
//�ð��� ���ߴ� ��ũ��Ʈ�� TimeScaleManager�� ����
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
