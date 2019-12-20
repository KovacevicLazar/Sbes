using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Server
{
    public class WCFService : IWCFContracts
	{
		private string secretKey;
        public static List<String> primljenePoruke = new List<string>();

		public void SetSecretKey(string key)
		{
			secretKey = key; 
		}

		public void SendMessage(string message)
		{
			///TODO: implementirati dekripciju
			///string decryptedMessage = Decrypt(message, secretKey);


		}

		public void TestCommunication()
		{
			throw new NotImplementedException();
		}

        public string Read()
        {
            if (primljenePoruke.Count == 0)
            {
                return "dasdasdasdasdasdasd";
            }
            else
            {
                return primljenePoruke[primljenePoruke.Count-1];
            }
        }

        public bool Write(string text)
        {
            primljenePoruke.Add(text);
            Console.WriteLine("Klijent poslao: "+Decript( text, SecretKey.secretKey));
            return true;
        }


        public static string Decript(string input, string key)
        {
            byte[] byteBuff = Encoding.Unicode.GetBytes(input); //od umaznog stringa pravimo niz bajta
            byte[] decrypted; //pomocni niz u koji se dekriptuje

            byte[] KeyFor3DES = Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tripleDesCrypto = new TripleDESCryptoServiceProvider
            {
                Key = KeyFor3DES,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };

            tripleDesCrypto.IV = byteBuff.Take(tripleDesCrypto.BlockSize / 8).ToArray();               // take the iv off the beginning of the ciphertext message			
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


        public string Encript(string input, string key)
        {
            byte[] rawInput = Encoding.UTF8.GetBytes(input); //pravimo niz bajtova od unetog stringa
            byte[] encrypted; //pomocni niz u koga enkriptujemo

            TripleDESCryptoServiceProvider tripleDesCrypto = new TripleDESCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None
            };

            //Mora sa ovim algoritmom, sa starim radi samo u  ECB modu
            tripleDesCrypto.GenerateIV();
            ICryptoTransform tripleDesEncrypt = tripleDesCrypto.CreateEncryptor();

            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(mStream, tripleDesEncrypt, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(rawInput, 0, rawInput.Length);
                    encrypted = tripleDesCrypto.IV.Concat(mStream.ToArray()).ToArray();
                }
            }
            return Convert.ToBase64String(encrypted);
        }


    }
}
