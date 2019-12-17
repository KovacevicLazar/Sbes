using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Client.Exceptions;
using System.Security.Cryptography;

namespace Client
{
	public class WCFClientAuthenticator : ChannelFactory<IClientConnection>, IDisposable
	{
		IClientConnection factory;

		public WCFClientAuthenticator(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			factory = CreateChannel();
		}
		public Tuple<string, string> Connect(string username, string password, string service)
		{
			try
			{
				if (Authenticate(username, password))
				{
					Tuple<string, string> serviceEndpoint = factory.SendServiceRequest(service, username);
                    if (serviceEndpoint == null )
                    {
                        Console.WriteLine("Trazeni servis nije aktivan.");
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"Klijent konektovan na {serviceEndpoint.Item1}.");
                    }

                    string encriptSecretKey = serviceEndpoint.Item2;
                    string haspass = CreateSHA1("password");
                    string secretKey = Decript(encriptSecretKey, haspass);

                    serviceEndpoint.Item2.Replace(serviceEndpoint.Item2, secretKey);


                    if (serviceEndpoint != null)
					{
						Console.WriteLine($"'{serviceEndpoint}' pronadjen");
						return serviceEndpoint;
					}
					else
					{
						Console.WriteLine($"Servis '{service}' nije pronadjen.");
						return null;
					}
				}
				else
				{
					Console.WriteLine("Invalid password.");
					return null;
				}
			}
			catch (NoSuchUserException e)
			{
				Console.WriteLine(e.Message);
				return null; 
			}
		}

       
        /// <summary>
        /// Autentifikuje korisnika na osnovu prosledjenih kredencijala
        /// </summary>
        /// <param name="username">Windows ID usera</param>
        /// <param name="password"></param>
        /// <returns>Da li je korisnik validan; Exception ako ne postoji</returns>
        private bool Authenticate(string username, string password)
		{
			bool ret = false;
			try
			{
				ret = factory.ValidateUser(username, password);
			}
			catch (Exception e)
			{
				Console.WriteLine("[SendMessage] ERROR = {0}", e.Message);
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


        public string Decript(string input, string key)
        {
            byte[] byteKey = StringToByteArray(key);
            byte[] buffer = new byte[4] { 0, 0, 0, 0 };

            byte[] rv = new byte[byteKey.Length + buffer.Length];
            System.Buffer.BlockCopy(byteKey, 0, rv, 0, byteKey.Length);
            System.Buffer.BlockCopy(buffer, 0, rv, byteKey.Length, buffer.Length);

            TripleDESCryptoServiceProvider tripleDesCrypto = new TripleDESCryptoServiceProvider
            {
                Key = rv,
                Mode =CipherMode.CBC,
                Padding = PaddingMode.None
            };

            byte[] byteBuff = Convert.FromBase64String(input);

            string plaintext = Encoding.UTF8.GetString(tripleDesCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }
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
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                //byte[] buffer = new byte[4] { 0, 0, 0, 0 };
                //sb.Append(Encoding.UTF8.GetString(buffer));
                Console.WriteLine(sb.ToString());
                return sb.ToString();
            }
        }


        //public void TestCommunication()
        //{
        //	try
        //	{
        //		factory.TestCommunication();
        //	}
        //	catch (Exception e)
        //	{
        //		Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
        //	}
        //}
    }
}
