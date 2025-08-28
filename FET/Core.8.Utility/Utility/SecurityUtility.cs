using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Utility.Utility
{
    /// <summary>
    /// 【加密、解密】
    /// </summary>
    public class SecurityUtility
    {   
        /// <summary>
        /// 加密字串(使用AES Algo)
        /// </summary>
        /// <param name="str">被加密字串</param>
        /// <param name="key">金鑰(長度需為16)</param>
        /// <param name="iv">轉置(長度需為32)</param>
        /// <returns>加密後的字串</returns>
        public static string Encrypt(string str,string key = "",string iv = "")
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
            string encryptString = "";
            using (Aes rDel = Aes.Create()) {
                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.Zeros;
                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                encryptString = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            return encryptString;
        }

        /// <summary>
        /// 解密字串(使用AES)
        /// </summary>
        /// <param name="str">被加密字串</param>
        /// <param name="key">金鑰(長度需為16)</param>
        /// <param name="iv">轉置(長度需為32)</param>
        /// <returns>解密後的字串</returns>
        public static string Decrypt(string str, string key = "", string iv = "")
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Convert.FromBase64String(str);
            string decryptString = "";
            using (Aes rDel = Aes.Create())
            {
                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.Zeros;
                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                decryptString = Encoding.UTF8.GetString(resultArray).Replace("\0", "");
            }
            return decryptString;
        }


        /// <summary>
        /// 雜奏字串(使用Pbkdf2+SHA256)
        /// </summary>
        /// <param name="saltString">加鹽</param>
        /// <param name="text">要湊的字串</param>
        /// <returns>雜湊後字串</returns>
        public static string PbkSHAF2Hash(string saltString, string text)
        {
            byte[] salt = Encoding.ASCII.GetBytes(saltString);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: text,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        /// <summary>
        /// 雜奏字串(使用MD5)
        /// </summary>
        /// <param name="input">要湊的字串</param>
        /// <returns>雜湊結果字串</returns>
        public static string MD5Hash(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}