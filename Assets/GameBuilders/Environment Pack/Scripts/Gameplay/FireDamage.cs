//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using System;
using System.Collections.Generic;
using System.Linq;
using GameBuilders.FPSBuilder.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FPSBuilder.ModernWeaponsPack
{
    public class FireDamage : MonoBehaviour
    {
        [SerializeField]
        [MinMax(0.5f, 3)]
        private float m_DamageRate = 2;
        
        [SerializeField]
        [MinMax(0.001f, Mathf.Infinity)]
        private float m_MinDamage = 20.0f;
        
        [SerializeField]
        [MinMax(0.001f, Mathf.Infinity)]
        private float m_MaxDamage = 35.0f;
        
        private Dictionary<int, Tuple<IDamageable, Vector3, float>> m_Targets = new Dictionary<int, Tuple<IDamageable, Vector3, float>>();

        private void OnTriggerEnter(Collider c)
        {
            IDamageable damageableTarget = c.GetComponent<IDamageable>();
            
            if (damageableTarget != null)
                m_Targets.Add(c.GetInstanceID(), new Tuple<IDamageable, Vector3, float>(damageableTarget, c.transform.position, Time.time + m_DamageRate * 0.5f));
        }
        
        private void OnTriggerExit(Collider c)
        {
            m_Targets.Remove(c.GetInstanceID());
        }

        private void Update()
        {
            for (int i = 0, l = m_Targets.Count; i < l; i++)
            {
                int key = m_Targets.ElementAt(i).Key;
                Tuple<IDamageable, Vector3, float> target = m_Targets[key];

                if (target.Item1.IsAlive)
                {
                    if (Time.time > target.Item3)
                    {
                        target.Item1?.Damage(Random.Range(m_MinDamage, m_MaxDamage), transform.position, target.Item2);
                        m_Targets[key] = new Tuple<IDamageable, Vector3, float>(target.Item1, target.Item2, Time.time + m_DamageRate);
                    }
                }
                else
                {
                    m_Targets.Remove(key);
                    return;
                }
            }
        }
    }
}
