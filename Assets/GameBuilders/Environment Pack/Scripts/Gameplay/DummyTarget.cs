//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Interfaces;
using UnityEngine;

namespace FPSBuilder.ModernWeaponsPack
{
    public class DummyTarget : MonoBehaviour, IProjectileDamageable, IExplosionDamageable
    {
        private Animator m_Animator;

        public bool IsAlive => false;

        private void Start ()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void ProjectileDamage (float damage, Vector3 targetPosition, Vector3 hitPosition, float penetrationPower)
        {
            if (m_Animator)
            {
                m_Animator.Play("TargetHit");
            }
        }

        public void Damage (float damage)
        {
            if (m_Animator)
            {
                m_Animator.Play("TargetHit");
            }
        }

        public void Damage (float damage, Vector3 targetPosition, Vector3 hitPosition)
        {
            if (m_Animator)
            {
                m_Animator.Play("TargetHit");
            }
        }

        public void ExplosionDamage (float damage, Vector3 targetPosition, Vector3 hitPosition)
        {
            if (m_Animator)
            {
                m_Animator.Play("TargetHit");
            }
        }
    }
}
