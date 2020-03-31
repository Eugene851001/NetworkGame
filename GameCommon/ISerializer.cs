using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    interface ISerializer
    {
        byte[] Serialize(PlayerInfo message);
        PlayerInfo Deserialize(byte[] data, int amount);
    }
}
