//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using UnityEngine;

namespace GameBuilders.MinimalistUI.Scripts.DamageIndicator
{
    public class Indicator : MonoBehaviour
    {
        public Vector3 TargetPosition
        {
            get; 
            set;
        }

        public float Alpha
        {
            get; 
            set;
        }

        public float GetAngleRelativeFromTransform (Transform t)
        {
            Vector3 direction = (TargetPosition - t.position).normalized;
            return Mathf.Atan2(direction.x, direction.z) * -Mathf.Rad2Deg + t.eulerAngles.y - 270;
        }
    }
}
