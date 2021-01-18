namespace YPS.CommonClasses
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using YPS.Helpers;

    /// <summary>
    /// Encrypt Manager
    /// </summary>
    public class EncryptManager
    {
        #region Data members declaration
        private static string passPhrase = "Pas5pr@se";
        private static string saltValue = "s@1tValue";
        private static string hashAlgorithm = "SHA1";
        private static int passwordIterations = 2;
        private static string initVector = "@1B2c3D4e5F6g7H8";
        private static int keySize = 256;
        #endregion

        /// <summary>
        /// Decrypt Data
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            return Decrypt(cipherText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        }


        private static string Decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            try
            {
                byte[] initVectorBytes;
                initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes;
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] cipherTextBytes;
                cipherTextBytes = Convert.FromBase64String(cipherText);

                PasswordDeriveBytes password;
                password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
                byte[] keyBytes;
                keyBytes = password.GetBytes(keySize / 8);
                RijndaelManaged symmetricKey;
                symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor;
                decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream;
                memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream;
                cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                int cipherCount = cipherTextBytes.Length;
                byte[] plainTextBytes;
                plainTextBytes = new byte[cipherCount];
                // TODO: NotImplemented statement: ICSharpCode.SharpRefactory.Parser.AST.VB.ReDimStatement 
                int decryptedByteCount;
                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                string plainText;
                plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                return plainText;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Decrypt -> in EncryptManager.cs " + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Encrypt Data
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            return Encrypt(plainText, passPhrase, saltValue, hashAlgorithm, passwordIterations, initVector, keySize);
        }

        private static string Encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            try
            {
                byte[] initVectorBytes;
                initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes;
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] plainTextBytes;
                plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                PasswordDeriveBytes password;
                password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
                byte[] keyBytes;
                keyBytes = password.GetBytes(keySize / 8);
                RijndaelManaged symmetricKey;
                symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor;
                encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream;
                memoryStream = new MemoryStream();
                CryptoStream cryptoStream;
                cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes;
                cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                string cipherText;
                cipherText = Convert.ToBase64String(cipherTextBytes);
                return cipherText;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Encrypt -> in EncryptManager.cs " + Settings.userLoginID);
                return null;
            }
            
        }
    }
}
