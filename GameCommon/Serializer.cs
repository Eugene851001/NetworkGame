using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace GameCommon
{
    public class Serializer : ISerializer
    {
        public byte[] Serialize(PlayerInfo message)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PlayerInfo), new Type[]{ typeof(PlayerInfo) });
            MemoryStream messageContainer = new MemoryStream();
            serializer.Serialize(messageContainer, message);
            return messageContainer.GetBuffer();
        }

        public PlayerInfo Deserialize(byte[] data, int amount)
        {
            MemoryStream messageContainer = new MemoryStream();
            messageContainer.Write(data, 0, amount);
            XmlSerializer serializer = new XmlSerializer(typeof(PlayerInfo), new Type[] { typeof(PlayerInfo)});
            messageContainer.Position = 0;
            PlayerInfo message = (PlayerInfo)serializer.Deserialize(messageContainer);
            return message;
        }
    }
}
