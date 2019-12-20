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
                return "";
            }
            else
            {
                string posljednjaPorukaIzListe = primljenePoruke[primljenePoruke.Count - 1];
                return Encrypt(posljednjaPorukaIzListe,SecretKey.secretKey);
            }
        }

        public bool Write(string text)
        {
            string decriptText = Decript(text, SecretKey.secretKey);
            primljenePoruke.Add(decriptText);
            Console.WriteLine("Klijent poslao: {0}, Dekriptovano: {1}",text,decriptText);
            return true;
        }


        public static string Decript(string input, string key)
        {
            //while (input.Length % 8 != 0)    //kriptuje samo do velicine deljuve sa 8 ostatak odbaci, zbog toga ova petlja
            //{
            //    input = input + " ";
            //}

            byte[] byteBuff = Convert.FromBase64String(input); //od umaznog stringa pravimo niz bajta
            byte[] decrypted; //pomocni niz u koji se dekriptuje

            //Pravimo Kljuc za 3DES
            byte[] KeyFor3DES = ASCIIEncoding.ASCII.GetBytes(key);

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


        public string Encrypt(string input, string key)
        {
            //while (input.Length % 8 != 0)    //kriptuje samo do velicine deljuve sa 8 ostatak odbaci, zbog toga ova petlja
            //{
            //    input = input + " ";
            //}
            byte[] rawInput = Encoding.UTF8.GetBytes(input); //pravimo niz bajtova od unetog stringa
            byte[] encrypted; //pomocni niz u koga enkriptujemo

            //pravimo kljuc za 3DES
            byte[] KeyFor3DES = ASCIIEncoding.ASCII.GetBytes(key);

            TripleDESCryptoServiceProvider tripleDesCrypto = new TripleDESCryptoServiceProvider
            {
                Key = KeyFor3DES,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.Zeros
            };
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
