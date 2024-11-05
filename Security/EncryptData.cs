using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace EasyPaperWork.Security
{
    public class EncryptData
    {
        public EncryptData()
        {


        }
        public byte[] GetKey(byte[] salt, string password)
        {
            byte[] key = GenerateKeyFromPasswordAndSalt(password, salt);
            return key;
        }
        public byte[] GetSaltBytes(string salt_string)
        {
            //Conversão desfeita pois estes caracteres são sensíveis ao firebase
            salt_string = salt_string.Replace("@", "/")
                             .Replace("#", "+")
                             .Replace("$", "=");

            return Convert.FromBase64String(salt_string);

        }



        public string GenerateSaltString()
        {
            byte[] salt = new byte[32];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            string saltstring = Convert.ToBase64String(salt);
            //Conversão feita pois estes caracteres são sensíveis ao firebase
            saltstring = saltstring.Replace("/", "@")
                                         .Replace("+", "#")
                                         .Replace("=", "$");
            return saltstring;
        }

        // Renomear o método de EncryptData para Encrypt
        public string EncryptObject(object obj, byte[] key, byte[] salt)
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
            string EncryptString = Convert.ToBase64String(encryptedBytes);

            // Conversão feita pois estes caracteres são sensíveis ao Firebase
            EncryptString = EncryptString.Replace("/", "@")
                                         .Replace("+", "#")
                                         .Replace("=", "$");

            return EncryptString;
        }

        // Renomear o método de DecryptData para Decrypt
        public string Decrypt(string cipherText, byte[] key, byte[] salt)
        {
            //Conversão desfeita pois estes caracteres são sensíveis ao firebase           
            cipherText = cipherText.Replace("@", "/")
                .Replace("#", "+")
                .Replace("$", "=");
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
        public async Task<string> EncryptFile(string inputFilePath, string outputFilePath, string password, byte[] salt)
        {
            try
            {
                await Task.Run(() =>
                {
                    var aes = Aes.Create();
                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 10000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    using (var fileStream = new FileStream(outputFilePath, FileMode.Create))
                    {
                        fileStream.Write(salt, 0, salt.Length);
                        using (var cryptoStream = new CryptoStream(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (var inputStream = new FileStream(inputFilePath, FileMode.Open))
                        {
                            inputStream.CopyTo(cryptoStream);
                        }
                    }
                });
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public void DecryptFile(string inputFilePath, string outputFilePath, string password)
        {
            var fileStream = new FileStream(inputFilePath, FileMode.Open);


            byte[] salt = new byte[32];
            fileStream.Read(salt, 0, salt.Length);

            // Derivar a chave e IV a partir da senha e salt
            var aes = Aes.Create();
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 10000);
            aes.Key = key.GetBytes(aes.KeySize / 8);
            aes.IV = key.GetBytes(aes.BlockSize / 8);

            var cryptoStream = new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            var outputStream = new FileStream(outputFilePath, FileMode.Create);

            // Descriptografar o conteúdo do arquivo
            cryptoStream.CopyTo(outputStream);

            outputStream.Close();
            cryptoStream.Close();
            fileStream.Close();
        }
        public byte[] GenerateKeyFromPasswordAndSalt(string password, byte[] salt, int keySize = 32)
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
