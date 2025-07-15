//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using UnityEngine;

namespace GameBuilders.FPSBuilder.Interfaces
{
    public interface IWeaponAnimator
    {
        void Draw();
        
        void Hide();
        
        void Hit(Vector3 position);
        
        void Interact();
        
        void Vault();
    }
}