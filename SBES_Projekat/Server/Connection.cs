using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Server
{
    public class Connection : IServiceKeyHandler
    {
        public void SendEncriptedSecretKey(string key)
        {
            string haspass = CreateSHA1("password");
            SecretKey.secretKey = Decript(key, haspass);
        }


        public string Decript(string input, string key)
        {
            byte[] byteBuff = Convert.FromBase64String(input); //od umaznog stringa pravimo niz bajta
            byte[] decrypted; //pomocni niz u koji se dekriptuje

            //Pravimo Kljuc za 3DES
            byte[] byteKey = StringToByteArray(key);
            byte[] buffer = new byte[4] { 0, 0, 0, 0 }; 
            byte[] KeyFor3DES = new byte[byteKey.Length + buffer.Length];
            System.Buffer.BlockCopy(byteKey, 0, KeyFor3DES, 0, byteKey.Length);
            System.Buffer.BlockCopy(buffer, 0, KeyFor3DES, byteKey.Length, buffer.Length);

            TripleDESCryptoServiceProvider tripleDesCrypto = new TripleDESCryptoServiceProvider
            {
                Key = KeyFor3DES,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };

            tripleDesCrypto.IV = byteBuff.Take(tripleDesCrypto.BlockSize / 8).ToArray();                // take the iv off the beginning of the ciphertext message			
            ICryptoTransform tripleDesDecrypt = tripleDesCrypto.CreateDecryptor();

            using (MemoryStream mStream = new MemoryStream(byteBuff.Skip(tripleDesCrypto.BlockSize / 8).ToArray()))
            {
                using (CryptoStream cryptoStream = new CryptoStream(mStream, tripleDesDecrypt, CryptoStreamMode.Read))
                {
                    decrypted = new byte[byteBuff.Length - tripleDesCrypto.BlockSize / 8];     
                    cryptoStream.Read(decrypted, 0, decrypted.Length);
                }
            }
            string plaintext = Encoding.UTF8.GetString(decrypted);
            return plaintext;
        }

        
        /// Convert the hexadecimal string to byte array
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }


        public static string CreateSHA1(string input)
        {
            // Use input string to calculate SHA1 hash
            using (System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                int i;
                StringBuilder sb = new StringBuilder();
                for (i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
