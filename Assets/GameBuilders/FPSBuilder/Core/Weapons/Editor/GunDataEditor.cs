//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace GameBuilders.FPSBuilder.Core.Weapons.Editor
{
    [CustomEditor(typeof(GunData))]
    public sealed class GunDataEditor : UnityEditor.Editor
    {
        private SerializedProperty m_GunName;
        private SerializedProperty m_WeaponType;
        private SerializedProperty m_DroppablePrefab;
        private SerializedProperty m_Weight;
        private SerializedProperty m_Size;
        private SerializedProperty m_MeleeDamage;
        private SerializedProperty m_MeleeForce;

        private SerializedProperty m_PrimaryFireMode;
        private SerializedProperty m_SecondaryFireMode;
        private SerializedProperty m_PrimaryRateOfFire;
        private SerializedProperty m_SecondaryRateOfFire;
        
        private SerializedProperty m_Range;

        private SerializedProperty m_BulletsPerShoot;
        private SerializedProperty m_BulletsPerBurst;

        private SerializedProperty m_AffectedLayers;

        private SerializedProperty m_AmmoType;
        private SerializedProperty m_ReloadMode;
        private SerializedProperty m_RoundsPerMagazine;
        private SerializedProperty m_HasChamber;

        private SerializedProperty m_MaximumSpread;
        private SerializedProperty m_BaseAccuracy;
        private SerializedProperty m_HIPAccuracy;
        private SerializedProperty m_AIMAccuracy;
        private SerializedProperty m_DecreaseRateByShooting;
        private SerializedProperty m_DecreaseRateByWalking;

        private void OnEnable ()
        {
            m_GunName = serializedObject.FindProperty("m_GunName");
            m_WeaponType = serializedObject.FindProperty("m_WeaponType");
            m_DroppablePrefab = serializedObject.FindProperty("m_DroppablePrefab");
            m_Weight = serializedObject.FindProperty("m_Weight");
            m_Size = serializedObject.FindProperty("m_Size");
            m_MeleeDamage = serializedObject.FindProperty("m_MeleeDamage");
            m_MeleeForce = serializedObject.FindProperty("m_MeleeForce");

            m_PrimaryFireMode = serializedObject.FindProperty("m_PrimaryFireMode");
            m_SecondaryFireMode = serializedObject.FindProperty("m_SecondaryFireMode");
            m_PrimaryRateOfFire = serializedObject.FindProperty("m_PrimaryRateOfFire");
            m_SecondaryRateOfFire = serializedObject.FindProperty("m_SecondaryRateOfFire");
            m_Range = serializedObject.FindProperty("m_Range");
            m_BulletsPerShoot = serializedObject.FindProperty("m_BulletsPerShoot");
            m_BulletsPerBurst = serializedObject.FindProperty("m_BulletsPerBurst");

            m_AmmoType = serializedObject.FindProperty("m_AmmoType");
            m_ReloadMode = serializedObject.FindProperty("m_ReloadMode");
            m_RoundsPerMagazine = serializedObject.FindProperty("m_RoundsPerMagazine");
            m_HasChamber = serializedObject.FindProperty("m_HasChamber");
            
            m_AffectedLayers = serializedObject.FindProperty("m_AffectedLayers");

            m_MaximumSpread = serializedObject.FindProperty("m_MaximumSpread");
            m_BaseAccuracy = serializedObject.FindProperty("m_BaseAccuracy");
            m_HIPAccuracy = serializedObject.FindProperty("m_HIPAccuracy");
            m_AIMAccuracy = serializedObject.FindProperty("m_AIMAccuracy");
            m_DecreaseRateByShooting = serializedObject.FindProperty("m_DecreaseRateByShooting");
            m_DecreaseRateByWalking = serializedObject.FindProperty("m_DecreaseRateByWalking");
        }

        public override void OnInspectorGUI ()
        {
            //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
            serializedObject.Update();

            EditorGUI.indentLevel = 0;
            EditorUtilities.FoldoutHeader("General Settings", m_GunName);

            if (m_GunName.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(m_GunName, new GUIContent("Name"));
                EditorGUILayout.PropertyField(m_WeaponType);
                EditorGUILayout.PropertyField(m_DroppablePrefab);

                EditorGUILayout.PropertyField(m_Weight, new GUIContent("Weight (kg)"));
                EditorGUILayout.PropertyField(m_Size, new GUIContent("Length (m)"));

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_MeleeDamage);
                EditorGUILayout.PropertyField(m_MeleeForce);
            }

            EditorGUI.indentLevel = 0;
            EditorUtilities.FoldoutHeader("Shooting Settings", m_PrimaryFireMode);

            if (m_PrimaryFireMode.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(m_PrimaryFireMode);
                EditorGUILayout.PropertyField(m_PrimaryRateOfFire);

                EditorGUILayout.PropertyField(m_SecondaryFireMode);

                if ((GunData.FireMode)m_SecondaryFireMode.enumValueIndex != GunData.FireMode.None)
                    EditorGUILayout.PropertyField(m_SecondaryRateOfFire);
                
                EditorGUILayout.Space();
                
                EditorGUILayout.PropertyField(m_Range);

                if (m_Range.floatValue <= 0)
                    EditorGUILayout.HelpBox("Range must be greater than 0.", MessageType.Warning);

                if ((GunData.FireMode)m_PrimaryFireMode.enumValueIndex == GunData.FireMode.ShotgunAuto
                    || (GunData.FireMode)m_PrimaryFireMode.enumValueIndex == GunData.FireMode.ShotgunSingle
                    || (GunData.FireMode)m_SecondaryFireMode.enumValueIndex == GunData.FireMode.ShotgunAuto
                    || (GunData.FireMode)m_SecondaryFireMode.enumValueIndex == GunData.FireMode.ShotgunSingle)
                {
                    EditorGUILayout.PropertyField(m_BulletsPerShoot);

                    if (m_BulletsPerShoot.intValue < 1)
                        EditorGUILayout.HelpBox("Bullets per shot must be greater than 0.", MessageType.Warning);
                }

                if ((GunData.FireMode)m_PrimaryFireMode.enumValueIndex == GunData.FireMode.Burst
                    || (GunData.FireMode)m_SecondaryFireMode.enumValueIndex == GunData.FireMode.Burst)
                {
                    EditorGUILayout.PropertyField(m_BulletsPerBurst);

                    if (m_BulletsPerBurst.intValue < 1)
                        EditorGUILayout.HelpBox("Bullets per burst must be greater than 0.", MessageType.Warning);
                }

                EditorGUILayout.PropertyField(m_AffectedLayers);
            }

            EditorGUI.indentLevel = 0;
            EditorUtilities.FoldoutHeader("Magazine Settings", m_ReloadMode);

            if (m_ReloadMode.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(m_AmmoType);
                EditorGUILayout.PropertyField(m_ReloadMode);

                EditorGUILayout.PropertyField(m_RoundsPerMagazine);

                if (m_RoundsPerMagazine.intValue < 1)
                    EditorGUILayout.HelpBox("Bullets per magazine must be greater than 0.", MessageType.Warning);

                EditorGUILayout.PropertyField(m_HasChamber);
            }

            EditorGUI.indentLevel = 0;
            EditorUtilities.FoldoutHeader("Accuracy Settings", m_HIPAccuracy);

            if (m_HIPAccuracy.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.PropertyField(m_MaximumSpread);
                EditorGUILayout.PropertyField(m_BaseAccuracy, new GUIContent("Minimum Accuracy"));
                EditorGUILayout.PropertyField(m_HIPAccuracy);
                EditorGUILayout.PropertyField(m_AIMAccuracy);
                EditorGUILayout.PropertyField(m_DecreaseRateByShooting);
                EditorGUILayout.PropertyField(m_DecreaseRateByWalking);
            }
            
            EditorUtilities.DrawSplitter();
            
            //Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI
            serializedObject.ApplyModifiedProperties();
        }
    }
}