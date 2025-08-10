using GameBuilders.FPSBuilder.Core.Weapons;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionManager : MonoBehaviour
{
    [Header("무기 프리팹")]
    [SerializeField]
    private Gun vectorPrefab;
    [SerializeField]
    private Gun spasPrefab;
    [SerializeField]
    private Gun cs5Prefab;

    [Header("UI 버튼")]
    [SerializeField]
    private Button vectorButton;
    [SerializeField]
    private Button spasButton;
    [SerializeField]
    private Button cs5Button;

    public static event Action<Gun> OnWeaponSelected;

    void Start()
    {
        //각 버튼 리스너 할당
        vectorButton.onClick.AddListener(() => SelectWeapon(vectorPrefab));
        spasButton.onClick.AddListener(() => SelectWeapon(spasPrefab));
        cs5Button.onClick.AddListener(() => SelectWeapon(cs5Prefab));
    }

    private void SelectWeapon(Gun weaponPrefab)
    {
        if (weaponPrefab != null)
            OnWeaponSelected?.Invoke(weaponPrefab);
        else
            Debug.LogError("무기 프리팹이 할당되지 않았습니다!");
    }
}
