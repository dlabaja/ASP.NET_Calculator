using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ASP.NET_Calculator.Models
{
    public class SessionManager
    {
        public static async Task<int> GenerateUID()
        {
            var uid = new Random().Next(int.MaxValue);
            while (await Firebird.GetValueCount("UID", uid.ToString()) != 0)
            {
                uid = new Random().Next(int.MaxValue);
            }
            return uid;
        }

        private static byte[] key = Encoding.UTF8.GetBytes(Secrets.secretKey);

        public static string EncryptCookie(string plaintext)
        {
            StringBuilder ciphertext = new StringBuilder();

            for (int i = 0; i < plaintext.Length; i++)
            {
                int plaintextChar = plaintext[i];
                int keyChar = key[i % key.Length];
                int encryptedChar = plaintextChar ^ keyChar;
                ciphertext.Append((char)encryptedChar);
            }

            return ciphertext.ToString();
        }

        public static string DecryptCookie(string ciphertext)
        {
            StringBuilder plaintext = new StringBuilder();

            for (int i = 0; i < ciphertext.Length; i++)
            {
                int encryptedChar = ciphertext[i];
                int keyChar = key[i % key.Length];
                int plaintextChar = encryptedChar ^ keyChar;
                plaintext.Append((char)plaintextChar);
            }

            return plaintext.ToString();
        }
    }
}