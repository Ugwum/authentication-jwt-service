using Prospa.AuthService.Core.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Utils
{
    public class CryptoHelper
    {
        public static string Encrypt(string input, string privateKey)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(input);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportFromPem(privateKey);
               // rsa.rFromXmlString(privateKey);
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(encryptedData);
            }
        }

        public static string Decrypt(string input, string privateKey)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(input);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportFromPem(privateKey);
                //rsa.FromXmlString(privateKey);
                byte[] decryptedData = rsa.Decrypt(dataToDecrypt, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
    }
}
