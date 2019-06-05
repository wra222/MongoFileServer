using System.IO;

namespace MonGo.Entity
{
    public class MD5Helper
    {
        public static string GetMD5Hash(Stream stream)
        {
            string result = "";
            string hashData = "";
            byte[] arrbytHashValue;
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher =
                       new System.Security.Cryptography.MD5CryptoServiceProvider();

            try
            {
                //将stream Position 更换到最开始
                stream.Position = 0;
                arrbytHashValue = md5Hasher.ComputeHash(stream);//计算指定Stream 对象的哈希值
                                                                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                hashData = System.BitConverter.ToString(arrbytHashValue);
                //替换-
                hashData = hashData.Replace("-", "");
                result = hashData;
            }
            catch (System.Exception ex)
            {
                //记录日志
            }

            return result;
        }
    }
}
