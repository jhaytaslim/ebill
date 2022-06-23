using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;


namespace ebill.Utils
{
    public class AESThenHMAC
    {
        private static String key = "aesEncryptionKey";
        private static String initVector = "encryptionIntVec";
        private static readonly int BlockBitSize = 128;
        private static readonly int KeyBitSize = 128;

        private static AesManaged CreateAes(string ckey = "aesEncryptionKey", string cinitVector = "encryptionIntVec")
        {
            key = ckey ?? key;
            initVector = cinitVector ?? initVector;
            var aes = new AesManaged();
            aes.Key = System.Text.Encoding.UTF8.GetBytes(key); //UTF8-Encoding
            aes.IV = System.Text.Encoding.UTF8.GetBytes(initVector);//UT8-Encoding
            return aes;
        }

        public static string Encrypt(string text, string key = null, string initVector = null)
        {
            using (AesManaged aes = CreateAes(key, initVector))
            {
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(text);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
        
        public static string Decrypt(string text, string key = null, string initVector = null)
        {
            using (var aes = CreateAes(key, initVector))
            {
                ICryptoTransform decryptor = aes.CreateDecryptor();
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(text)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }

            }
        }
        
        public static string IV()
        {
            int length = BlockBitSize / 8;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

        public static string Key()
        {
            int length = KeyBitSize / 8;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }


    }
}