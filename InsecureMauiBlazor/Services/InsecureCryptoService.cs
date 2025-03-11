using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InsecureMauiBlazor.Services
{
    public class InsecureCryptoService : ICryptoService
    {
        // VULNERABILITY: Hardcoded encryption key
        private static readonly byte[] HardcodedKey = Encoding.UTF8.GetBytes("ThisIsAWeakHardcodedKey123");

        // VULNERABILITY: Hardcoded initialization vector
        private static readonly byte[] HardcodedIV = Encoding.UTF8.GetBytes("InitVectorFixed!");

        public string Encrypt(string plainText)
        {
            return InsecureEncrypt(plainText);
        }

        public string Decrypt(string cipherText)
        {
            return InsecureDecrypt(cipherText);
        }

        public string Hash(string input)
        {
            return InsecureHash(input);
        }

        // VULNERABILITY: Weak encryption implementation
        private string InsecureEncrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                // VULNERABILITY: Using Obsolete DES algorithm
#pragma warning disable SYSLIB0021
                using (DES des = DES.Create())
                {
                    des.Key = HardcodedKey.Take(8).ToArray(); // DES uses 8 bytes
                    des.IV = HardcodedIV.Take(8).ToArray();

                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputBytes, 0, inputBytes.Length);
                            cs.FlushFinalBlock();
                        }

                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
#pragma warning restore SYSLIB0021
            }
            catch
            {
                // VULNERABILITY: Swallowing exceptions
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            }
        }

        // VULNERABILITY: Weak decryption implementation
        private string InsecureDecrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                // VULNERABILITY: Using Obsolete DES algorithm
#pragma warning disable SYSLIB0021
                using (DES des = DES.Create())
                {
                    des.Key = HardcodedKey.Take(8).ToArray();
                    des.IV = HardcodedIV.Take(8).ToArray();

                    byte[] cipherBytes = Convert.FromBase64String(cipherText);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.FlushFinalBlock();
                        }

                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
#pragma warning restore SYSLIB0021
            }
            catch
            {
                // VULNERABILITY: Swallowing exceptions and fallback
                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
                }
                catch
                {
                    return cipherText; // Just return the original text on any error
                }
            }
        }

        // VULNERABILITY: Using weak hash algorithm (MD5)
        private string InsecureHash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // VULNERABILITY: Using MD5
#pragma warning disable SYSLIB0021
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
#pragma warning restore SYSLIB0021
        }
    }
}
