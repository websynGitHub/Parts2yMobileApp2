using System;

namespace YPS.CommonClasses
{
    public class Helperclass
    {
        #region Password Encryption and Decryption
        public static string Encrypt(string value)
        {
            string strReturn = string.Empty;
            
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    byte[] encData_byte = new byte[value.Length];
                    encData_byte = System.Text.Encoding.UTF8.GetBytes(value);
                    strReturn = Convert.ToBase64String(encData_byte);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return strReturn;
        }

        /// <summary>
        /// Password Decrypt
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Decrypt(string value)
        {
            string strReturn = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                    System.Text.Decoder utf8Decode = encoder.GetDecoder();
                    byte[] todecode_byte = Convert.FromBase64String(value);
                    int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                    char[] decoded_char = new char[charCount];
                    utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                    strReturn = new String(decoded_char);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return strReturn;
        }
        #endregion
    }
}
