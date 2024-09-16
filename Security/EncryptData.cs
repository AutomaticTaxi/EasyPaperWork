using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace EasyPaperWork.Security
{
    public class EncryptData
    {
        public EncryptData() {
        
        
        }
        public byte[] GenerateKey(string salt_string,string password) {
            byte[] salt = Convert.FromBase64String(salt_string);
            byte[] key = GenerateKeyFromPasswordAndSalt(password, salt);
            return key;
        }
        

        
        public string GenerateSaltString()
        {
            byte[] salt = new byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        // Renomear o método de EncryptData para Encrypt
        public string EncryptObject(object obj, byte[] key, byte[]salt)
        {
            string json = JsonConvert.SerializeObject(obj);
            json = Encrypt(json, key, salt);
            return json;

        }
        public string Encrypt(string plainText, byte[] key, byte[] salt)
        {
            var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            byte[] iv = aes.IV;

            var encryptor = aes.CreateEncryptor(aes.Key, iv);
            var msEncrypt = new MemoryStream();
            msEncrypt.Write(iv, 0, iv.Length);
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            var swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(plainText);
            swEncrypt.Close();

            byte[] encryptedBytes = msEncrypt.ToArray();
            return Convert.ToBase64String(encryptedBytes);
        }

        // Renomear o método de DecryptData para Decrypt
        public string Decrypt(string cipherText, byte[] key, byte[] salt)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            var aes = Aes.Create();
            aes.Key = key;

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipherBytes = new byte[fullCipher.Length - iv.Length];
            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var msDecrypt = new MemoryStream(cipherBytes);
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        public  byte[] GenerateKeyFromPasswordAndSalt(string password, byte[] salt, int keySize = 32)
        {
            var sha256 = SHA256.Create();
            
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combinedBytes = new byte[passwordBytes.Length + salt.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, combinedBytes, passwordBytes.Length, salt.Length);

            return sha256.ComputeHash(combinedBytes);
            
        }
    }
}
