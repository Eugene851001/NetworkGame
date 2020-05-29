using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GameCommon
{
    public enum MoveRequest { Up, Down, Left, Right};

    public enum MessageType { AddPlayer, DeletePlayer, PlayerInfo, PlayerAction, 
        Chat, PersonalAddPlayer, ServerInfo, Regitsration, SearchRequest, RespawnRequest};

    [Serializable]
    abstract public class GameMessage
    {
        public MessageType MessageType;
        public int PlayerID;
    }
}
