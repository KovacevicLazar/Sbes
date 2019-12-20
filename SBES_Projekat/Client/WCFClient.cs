using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IO;

namespace Client
{
	public class WCFClient : ChannelFactory<IWCFContracts>, IDisposable
	{
		IWCFContracts factory;

		public WCFClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			factory = CreateChannel();
		}
		public bool Write(string message, string key)
		{
            bool ret;
			try
			{
				///TODO: enkripcija poruke pre slanja
				string encriptedMessage = Encrypt(message, key);
                ret=factory.Write(encriptedMessage);
                Console.WriteLine("ENKRIPTOVANA PORUKA POSLATA SERVERU:   "+encriptedMessage);
			}
			catch (Exception e)
			{
				Console.WriteLine("[Write] ERROR = {0}", e.Message);
                ret = false;
			}
            return ret;
		}

        public string Read(string key)
        {
            string ret;
            try
            {
                string decriptedMessage = factory.Read();
                Console.WriteLine("ENKRIPTOVANA Pprimljena sa servera:   " + decriptedMessage);
                ret = Decript(decriptedMessage, key);
                Console.WriteLine("Dekriptovana poruka sa servera" + ret);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Read] ERROR = {0}", e.Message);
                ret = "";
            }
            return ret;
        }

        public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}



        public static string Decript(string input, string key)
        {
            while (input.Length % 8 != 0)    //kriptuje samo do velicine deljuve sa 8 ostatak odbaci, zbog toga ova petlja
            {
                input = input + " ";
            }

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
                    decrypted = new byte[byteBuff.Length - tripleDesCrypto.BlockSize/8];
                    cryptoStream.Read(decrypted, 0, decrypted.Length);
                }
            }
            string plaintext = Encoding.UTF8.GetString(decrypted);
            return plaintext;
        }


        public string Encrypt(string input, string key)
        {
            while (input.Length % 8 != 0)    //kriptuje samo do velicine deljuve sa 8 ostatak odbaci, zbog toga ova petlja
            {
                input = input + " ";
            }
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
