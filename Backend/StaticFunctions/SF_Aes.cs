using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Backend.StaticFunctions
{
    public class SF_Aes
    {
        // BRON: https://gist.github.com/magicsih/be06c2f60288b54d9f52856feb96ce8c
        /*
         *  Aes aes = new Aes();
                    string encryptedBase64String = aes.EncryptToBase64String(mail);
                    string decryptedString = aes.DecryptFromBase64String(encryptedBase64String);
         */

        private static RijndaelManaged rijndael = new RijndaelManaged();
        private static System.Text.UnicodeEncoding unicodeEncoding = new UnicodeEncoding();

        private const int CHUNK_SIZE = 128;

        private void InitializeRijndael()
        {
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
        }

        /*public Aes()
        {
            InitializeRijndael();

            rijndael.KeySize = CHUNK_SIZE;
            rijndael.BlockSize = CHUNK_SIZE;

            rijndael.GenerateKey();
            rijndael.GenerateIV();
        }*/

        public SF_Aes()
        {
            InitializeRijndael();

            rijndael.Key = Convert.FromBase64String(Environment.GetEnvironmentVariable("AES_KEY"));
            rijndael.IV = Convert.FromBase64String(Environment.GetEnvironmentVariable("AES_IV"));
        }
        
        public SF_Aes(int cookie)
        {
            InitializeRijndael();

            rijndael.Key = Convert.FromBase64String(Environment.GetEnvironmentVariable("EAS_KEY_Cookie"));
            rijndael.IV = Convert.FromBase64String(Environment.GetEnvironmentVariable("EAS_IV_Cookie"));
        }

        public SF_Aes(byte[] key, byte[] iv)
        {
            InitializeRijndael();

            rijndael.Key = key;
            rijndael.IV = iv;
        }

        public string Decrypt(byte[] cipher)
        {
            ICryptoTransform transform = rijndael.CreateDecryptor();
            byte[] decryptedValue = transform.TransformFinalBlock(cipher, 0, cipher.Length);
            return unicodeEncoding.GetString(decryptedValue);
        }

        public string DecryptFromBase64String(string base64cipher)
        {
            return Decrypt(Convert.FromBase64String(base64cipher));
        }

        public byte[] EncryptToByte(string plain)
        {
            ICryptoTransform encryptor = rijndael.CreateEncryptor();
            byte[] cipher = unicodeEncoding.GetBytes(plain);
            byte[] encryptedValue = encryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return encryptedValue;
        }

        public string EncryptToBase64String(string plain)
        {
            return Convert.ToBase64String(EncryptToByte(plain));
        }

        public string GetKey()
        {
            return Convert.ToBase64String(rijndael.Key);
        }

        public string GetIV()
        {
            return Convert.ToBase64String(rijndael.IV);
        }
    }
}
