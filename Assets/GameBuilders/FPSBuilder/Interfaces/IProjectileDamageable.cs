﻿//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using UnityEngine;

namespace GameBuilders.FPSBuilder.Interfaces
{
    public interface IProjectileDamageable : IDamageable
    {
        void ProjectileDamage(float damage, Vector3 targetPosition, Vector3 hitPosition, float penetrationPower);
    }
}