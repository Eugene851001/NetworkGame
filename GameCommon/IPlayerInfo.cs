using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    public interface IPlayerInfo
    {
        Vector2D GetPosition();
        int GetHealth();
    }
}
