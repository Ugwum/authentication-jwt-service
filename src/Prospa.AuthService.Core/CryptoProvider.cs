using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core
{
    public class AESCryptoProviderExtension
    {
        public static string Encrypt(string plainText, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                // Generate a new initialization vector (IV) for each operation
                aesAlg.GenerateIV();

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        // Prepend the IV to the encrypted data for later decryption
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(byte[] cipherText, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                // Extract the IV from the beginning of the cipherText
                byte[] iv = new byte[aesAlg.IV.Length];
                Array.Copy(cipherText, iv, iv.Length);

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
                {
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText, iv.Length, cipherText.Length - iv.Length))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        public static string Encrypt(string plainText, string base64Key)
        {
            byte[] key = Convert.FromBase64String(base64Key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                // Generate a new initialization vector (IV) for each operation
                aesAlg.GenerateIV();

                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        // Prepend the IV to the encrypted data for later decryption
                        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(byte[] cipherText, string base64Key)
        {
            byte[] key = Convert.FromBase64String(base64Key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;

                // Extract the IV from the beginning of the cipherText
                byte[] iv = new byte[aesAlg.IV.Length];
                Array.Copy(cipherText, iv, iv.Length);

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
                {
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText, iv.Length, cipherText.Length - iv.Length))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
    public class RSACryptoProviderExtension
    {
        public static string EncryptWithPrivateKey(string input, string privateKey)
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

        public static string DecryptWithPrivateKey(string input, string privateKey)
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
