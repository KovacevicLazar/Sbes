using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Security.Cryptography;
using System.IO;

namespace Client
{
	public class Client
	{
		static void Main(string[] args)
		{
			// Debugger.Launch();
			//DESKTOP-IJMHSLM\Luka
			WindowsIdentity id = WindowsIdentity.GetCurrent();
			//Console.WriteLine(id.Name);
           
			NetTcpBinding binding = new NetTcpBinding();
			binding.Security.Mode = SecurityMode.Transport;
			binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

			string username = "DESKTOP-IJMHSLM\\Luka";

			string address = "net.tcp://localhost:";
			string servicePort = "9999";
			string authenticationPort = "9998";
			string authenticationService = "DomenController";
			string service = "WCFService";

			Tuple<string, string> serviceEndpointAndKey;
			string secretKey = null;


			using (WCFClientAuthenticator authenticator = new WCFClientAuthenticator(binding, new EndpointAddress(new Uri(address + authenticationPort + "/" + authenticationService))))
			{
                ///	Slanje ID klijenta, PASSWORD klijenta

                string haspass = CreateSHA1("password");
                serviceEndpointAndKey = authenticator.Connect(username, haspass, service);
                /// TODO: dekriptovanje tajnog kljuca

                string encriptSecretKey = serviceEndpointAndKey.Item2;
                secretKey =  Decript(encriptSecretKey, haspass);
               
                Console.WriteLine("Klijent dobio tajni kljuc: " + secretKey);

                serviceEndpointAndKey.Item2.Replace(serviceEndpointAndKey.Item2, secretKey);

                while (serviceEndpointAndKey == null) System.Threading.Thread.Sleep(100);
			}

            ///	TODO: Komunikacija sa servisom
            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(serviceEndpointAndKey.Item1))))
            {
                if (proxy.Write("msgdadsadasasasdasasdassssss dsad", secretKey))
                {
                    Console.WriteLine("Poruka je uspesno poslata. ");
                }
                string primljenaPoruka = proxy.Read(secretKey);
                Console.WriteLine("Server Poslao: "+primljenaPoruka);
     
            }
				/// Create a signature using SHA1 hash algorithm
				//byte[] signature = DigitalSignature.Create();
				//proxy.SendMessage();
				


				/// For the same message, create a signature based on the "wrongCertCN"
				//X509Certificate2 wrongSignCert = null;

				/// Create a signature using SHA1 hash algorithm
				//byte[] signature1 = DigitalSignature.Create();
				//proxy.SendMessage();

			Console.ReadLine();
		}

        public static string Decript(string input, string key)
        {
            
            byte[] byteBuff = Convert.FromBase64String(input); //od umaznog stringa pravimo niz bajta
            byte[] decrypted; //pomocni niz u koji se dekriptuje

            //Pravimo Kljuc za 3DES
            byte[] byteKey = StringToByteArray(key);
            byte[] buffer = new byte[4] { 0, 0, 0, 0 }; // za dodavanje 4 bajta koja nedostaju za 3des key.length
            byte[] KeyFor3DES = new byte[byteKey.Length + buffer.Length];
            System.Buffer.BlockCopy(byteKey, 0, KeyFor3DES, 0, byteKey.Length);
            System.Buffer.BlockCopy(buffer, 0, KeyFor3DES, byteKey.Length, buffer.Length);

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

        // Koncertovanje heksadecimalnog stringa u niz bajtova, dobijamo 20 bajtova
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }




        public static string CreateSHA1(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.SHA1 md5 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                int i;
                StringBuilder sb = new StringBuilder();
                for (i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2")); //x2 heksadecimalne vrednosti
                }
                //Console.WriteLine(sb.ToString());
                return sb.ToString();
            }
        }
    }
}
