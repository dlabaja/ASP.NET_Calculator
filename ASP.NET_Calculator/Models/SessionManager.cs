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

        public static string EncryptCookie(string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] keyBytes = key;

            byte[] encryptedBytes = new byte[textBytes.Length];

            for (int i = 0; i < textBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(textBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string DecryptCookie(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = key;

            byte[] decryptedBytes = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}