using GameBuilders.FPSBuilder.Editor.Utility;
using UnityEditor;

namespace GameBuilders.FPSBuilder.Core.Weapons.Editor
{
    [CustomEditor(typeof(AmmoType))]
    public class AmmoTypeEditor : UnityEditor.Editor
    {
        private SerializedProperty m_AmmunitionName;
        private SerializedProperty m_ProjectileMass;
        private SerializedProperty m_ProjectileSpeed;
        private SerializedProperty m_DamageMode;
        private SerializedProperty m_Damage;
        private SerializedProperty m_DamageFalloffCurve;
        private SerializedProperty m_MaximumImpactForce;
            
        private SerializedProperty m_CanPenetrate;
        private SerializedProperty m_PenetrationPower;
        private SerializedProperty m_Refraction;
        private SerializedProperty m_DensityInfluence;

        private SerializedProperty m_Ricochet;
        private SerializedProperty m_RicochetChance;
        private SerializedProperty m_MaxIncidentAngle;
        private SerializedProperty m_TrajectoryDeflection;
        private SerializedProperty m_RicochetDensityThreshold;

        private SerializedProperty m_Fragmentation;
        private SerializedProperty m_FragmentationChance;
        private SerializedProperty m_MaxFragments;
        private SerializedProperty m_FragmentScattering;
        private SerializedProperty m_FragmentationDensityThreshold;
        private SerializedProperty m_FragmentationDamageMultiplier;

        private void OnEnable()
        {
            m_AmmunitionName = serializedObject.FindProperty("m_AmmunitionName");
            m_ProjectileMass = serializedObject.FindProperty("m_ProjectileMass");
            m_ProjectileSpeed = serializedObject.FindProperty("m_ProjectileSpeed");
            m_DamageMode = serializedObject.FindProperty("m_DamageMode");
            m_Damage = serializedObject.FindProperty("m_Damage");
            m_DamageFalloffCurve = serializedObject.FindProperty("m_DamageFalloffCurve");
            m_MaximumImpactForce = serializedObject.FindProperty("m_MaximumImpactForce");

            m_CanPenetrate = serializedObject.FindProperty("m_CanPenetrate");
            m_PenetrationPower = serializedObject.FindProperty("m_PenetrationPower");
            m_Refraction = serializedObject.FindProperty("m_Refraction");
            m_DensityInfluence = serializedObject.FindProperty("m_DensityInfluence");

            m_Ricochet = serializedObject.FindProperty("m_Ricochet");
            m_RicochetChance = serializedObject.FindProperty("m_RicochetChance");
            m_MaxIncidentAngle = serializedObject.FindProperty("m_MaxIncidentAngle");
            m_TrajectoryDeflection = serializedObject.FindProperty("m_TrajectoryDeflection");
            m_RicochetDensityThreshold = serializedObject.FindProperty("m_RicochetDensityThreshold");

            m_Fragmentation = serializedObject.FindProperty("m_Fragmentation");
            m_FragmentationChance = serializedObject.FindProperty("m_FragmentationChance");
            m_MaxFragments = serializedObject.FindProperty("m_MaxFragments");
            m_FragmentScattering = serializedObject.FindProperty("m_FragmentScattering");
            m_FragmentationDensityThreshold = serializedObject.FindProperty("m_FragmentationDensityThreshold");
            m_FragmentationDamageMultiplier = serializedObject.FindProperty("m_FragmentationDamageMultiplier");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.LabelField(EditorUtilities.GetRect(36), "Ammunition Settings", Styling.headerLabel);
            
            EditorGUILayout.PropertyField(m_AmmunitionName);
            EditorGUILayout.PropertyField(m_ProjectileMass);
            EditorGUILayout.PropertyField(m_ProjectileSpeed);
            EditorGUILayout.PropertyField(m_MaximumImpactForce);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(m_DamageMode);
            EditorGUILayout.PropertyField(m_Damage);
            EditorGUILayout.PropertyField(m_DamageFalloffCurve);
            
            EditorGUI.LabelField(EditorUtilities.GetRect(36), "Ballistics", Styling.headerLabel);
            
            EditorUtilities.ToggleHeader("Penetration", m_CanPenetrate);

            if (m_CanPenetrate.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                using (new EditorGUI.DisabledScope(!m_CanPenetrate.boolValue))
                {
                    EditorGUILayout.PropertyField(m_PenetrationPower);
                    using (new EditorGUI.DisabledScope(m_PenetrationPower.floatValue == 0))
                    {
                        EditorGUILayout.PropertyField(m_Refraction);
                        EditorGUILayout.PropertyField(m_DensityInfluence);
                    }
                }
            }
            EditorGUI.indentLevel = 0;
            
            EditorUtilities.ToggleHeader("Ricochet", m_Ricochet);

            if (m_Ricochet.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                using (new EditorGUI.DisabledScope(!m_Ricochet.boolValue))
                {
                    EditorGUILayout.PropertyField(m_RicochetChance);
                    using (new EditorGUI.DisabledScope(m_RicochetChance.floatValue == 0))
                    {
                        EditorGUILayout.PropertyField(m_MaxIncidentAngle);
                        EditorGUILayout.PropertyField(m_TrajectoryDeflection);
                        EditorGUILayout.PropertyField(m_RicochetDensityThreshold);
                    }
                }
            }
            EditorGUI.indentLevel = 0;
            
            EditorUtilities.ToggleHeader("Fragmentation", m_Fragmentation);

            if (m_Fragmentation.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                using (new EditorGUI.DisabledScope(!m_Fragmentation.boolValue))
                {
                    EditorGUILayout.PropertyField(m_FragmentationChance);
                    using (new EditorGUI.DisabledScope(m_FragmentationChance.floatValue == 0))
                    {
                        EditorGUILayout.PropertyField(m_MaxFragments);
                        EditorGUILayout.PropertyField(m_FragmentScattering);
                        EditorGUILayout.PropertyField(m_FragmentationDensityThreshold);
                        EditorGUILayout.PropertyField(m_FragmentationDamageMultiplier);
                    }
                }
            }
            EditorGUI.indentLevel = 0;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
