using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AGV.Map.Core
{
    public static class Extension
    {
        public static T DeepCopy<T>(this T obj)
        {
            MemoryStream stream = obj.SerializeToStream();
            return DeserializeFromStream<T>(stream);
        }

        public static T DeserializeFromStream<T>(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object obj = formatter.Deserialize(stream);
            return (T)obj;
        }

        public static MemoryStream SerializeToStream(this object obj)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream;
        }
    }
}