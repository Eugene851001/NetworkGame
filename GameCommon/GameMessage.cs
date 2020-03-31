using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GameCommon
{

    public enum MoveRequest { Up, Down, Left, Right};

    [XmlInclude(typeof(PlayerInfo))]
    public class GameMessage
    {
        [XmlIgnore]
        public PlayerInfo playerInfo;

        /*public Vector2D position;

        public int HealthPoint;*/

        public Vector2D newDirection;
        public int d = 3;
        public string content;
    }
}
