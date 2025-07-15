//=========== Copyright (c) GameBuilders, All rights reserved. ================//

namespace GameBuilders.FPSBuilder.Interfaces
{
    public interface IActionable
    {
        bool RequiresAnimation
        {
            get;
        }

        void Interact();

        string Message();
    }
}
