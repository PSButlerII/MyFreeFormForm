using System.Security.Cryptography;
using System.Text;


namespace MyFreeFormForm.Helpers
{
    public class EncryptionHelper
    {
        public static string Encrypt(string clearText)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt); // Use static method to fill the byte array

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("mysecretkey12345"); // replace with your own key
                aes.IV = salt;
                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(clearText), 0, clearText.Length);
                    return Convert.ToBase64String(salt.Concat(encrypted).ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] encrypted = Convert.FromBase64String(cipherText);
            byte[] salt = encrypted.Take(16).ToArray();
            encrypted = encrypted.Skip(16).ToArray();
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes("mysecretkey12345"); // replace with your own key
                aes.IV = salt;
                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                    return Encoding.UTF8.GetString(decrypted);
                }
            }
        }


    }


}
