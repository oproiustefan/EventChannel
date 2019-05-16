using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace BrokerEvent.Framework.Services
{
    public static class Helpers
    {
        public static byte[] ReadFromStream(NetworkStream stream)
        {
            byte[] buffer = new byte[54];
            int i;

            i = stream.Read(buffer, 0, 54);
            var length = FromByteArray<int>(buffer);
            
            byte[] data = new byte[length];
            stream.Read(data, 0, length);           
            return data;
        }

        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                ms.Position = 0;
                return (T) bf.Deserialize(ms);
            }
        }

        public static void WriteToStream<T>(NetworkStream stream, T resource)
        {
            var bytes = Helpers.ToByteArray(resource);
            var length = bytes.Length;
            var bytesLength = Helpers.ToByteArray(length);

            stream.Write(bytesLength, 0, 54);
            stream.Write(bytes, 0, length);
        }
    }
}