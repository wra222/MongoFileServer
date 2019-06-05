using System.IO;

namespace MonGo.Entity
{
    public class StreamHelper
    {
        /// 将  Stream 转成 MemoryStream
        public MemoryStream StreamToMemoryStream(Stream instream)
        {
            instream.Position = 0;
            MemoryStream outstream = new MemoryStream();
            const int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int count = 0;
            while ((count = instream.Read(buffer, 0, bufferLen)) > 0)
            {
                outstream.Write(buffer, 0, count);
            }
            return outstream;
        }
        /// 将  Stream 转成 byte[] 
        public static byte[] StreamToBytes(Stream InStream)

        {
            byte[] bytes = new byte[InStream.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                InStream.Position=0;
                int read;
                while ((read = InStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    ms.Write(bytes, 0, read);
                }
                bytes = ms.ToArray();
            }
            return bytes;
        }

        /// 将 byte[] 转成 Stream

        public Stream BytesToStream(byte[] bytes)

        {

            Stream stream = new MemoryStream(bytes);
            return stream;

        }
    }
}
