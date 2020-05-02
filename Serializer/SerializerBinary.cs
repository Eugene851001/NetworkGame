using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SerializeHandler;
using System.Runtime.Serialization.Formatters.Binary;

namespace SerializeHandler
{
    public class SerializerBinary: ISerialize
    {
        public byte[] Serialize(object obj, Type type, Type[] types)
        {
            MemoryStream container = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(container, obj);
            return container.ToArray();
        }

        public object Deserialize(Stream source, Type type, Type[] types)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            source.Position = 0;
            object info = formatter.Deserialize(source);
            return info;
        }
    }
}
