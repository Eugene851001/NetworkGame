using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SerializeHandler
{
    public interface ISerialize
    {
        byte[] Serialize(object obj, Type type, Type[] types = null);
        object Deserialize(Stream source, Type type, Type[] types);
    }
}
