using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTCP.Client.Helpers
{
    public class FileHelper
    {
        public static byte[] SampleBytesFromFile(String filePath, int currentPosition, int bufferSize)
        {
            int length = bufferSize;
            FileStream fs = new FileStream(filePath, FileMode.Open);
            fs.Position = currentPosition;

            if (currentPosition + length > fs.Length)
            {
                length = (int)(fs.Length - currentPosition);
            }

            byte[] b = new byte[length];
            fs.Read(b, 0, length);
            fs.Dispose();
            return b;
        }

        public static long GetFileLength(String filePath)
        {
            FileInfo info = new FileInfo(filePath);
            return info.Length;
        }

        public static void AppendAllBytes(String filePath, byte[] bytes)
        {
            FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            fs.Write(bytes, 0, bytes.Length);
            fs.Dispose();
        }
    }
}
