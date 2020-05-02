using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace SerializeHandler
{
    public class SerializerXml: ISerialize
    {
        public byte[] Serialize(object obj, Type type, Type[] types = null)
        {

            MemoryStream container = new MemoryStream();
            XmlSerializer formatter;
            if (types == null)
                formatter = new XmlSerializer(type);
            else
                formatter = new XmlSerializer(type, types);
            formatter.Serialize(container, obj);
            return container.ToArray();
        }

        public object Deserialize(Stream source, Type type, Type[] types)
        {
            byte[] buffer = new byte[source.Length];
            source.Read(buffer, 0, buffer.Length);
            string testXml = Encoding.ASCII.GetString(buffer);
            source.Position = 0;
            XmlSerializer formatter = new XmlSerializer(type, types);
            object info;
            info = formatter.Deserialize(source);
            return info;
        }
    }
}
