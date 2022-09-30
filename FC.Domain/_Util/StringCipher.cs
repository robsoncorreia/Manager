using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FC.Domain._Util
{
    public static class StringCipher
    {
        public static string Decrypt(string cipherText, string passPhrase = "Pedro")
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(passPhrase))
            {
                return string.Empty;
            }
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            byte[] cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8 * 2).Take(cipherTextBytesWithSaltAndIv.Length - (Keysize / 8 * 2)).ToArray();

            using Rfc2898DeriveBytes password = new(passPhrase, saltStringBytes, DerivationIterations);

            byte[] keyBytes = password.GetBytes(Keysize / 8);
            RijndaelEngine engine = new(256);
            CbcBlockCipher blockCipher = new(engine);
            PaddedBufferedBlockCipher cipher = new(blockCipher, new Pkcs7Padding());
            KeyParameter keyParam = new(keyBytes);
            ParametersWithIV keyParamWithIV = new(keyParam, ivStringBytes, 0, 32);

            cipher.Init(false, keyParamWithIV);
            byte[] comparisonBytes = new byte[cipher.GetOutputSize(cipherTextBytes.Length)];
            int length = cipher.ProcessBytes(cipherTextBytes, comparisonBytes, 0);

            _ = cipher.DoFinal(comparisonBytes, length);
            //return Convert.ToBase64String(saltStringBytes.Concat(ivStringBytes).Concat(comparisonBytes).ToArray());

            int nullIndex = comparisonBytes.Length - 1;
            while (comparisonBytes[nullIndex] == 0)
            {
                nullIndex--;
            }

            comparisonBytes = comparisonBytes.Take(nullIndex + 1).ToArray();

            string result = Encoding.UTF8.GetString(comparisonBytes, 0, comparisonBytes.Length);

            return result;
        }

        public static string Encrypt(string plainText, string passPhrase = "Pedro")
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(passPhrase))
            {
                return string.Empty;
            }
            byte[] saltStringBytes = Generate256BitsOfRandomEntropy();
            byte[] ivStringBytes = Generate256BitsOfRandomEntropy();
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using Rfc2898DeriveBytes password = new(passPhrase, saltStringBytes, DerivationIterations);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            RijndaelEngine engine = new(256);
            CbcBlockCipher blockCipher = new(engine);
            PaddedBufferedBlockCipher cipher = new(blockCipher, new Pkcs7Padding());
            KeyParameter keyParam = new(keyBytes);
            ParametersWithIV keyParamWithIV = new(keyParam, ivStringBytes, 0, 32);

            cipher.Init(true, keyParamWithIV);
            byte[] comparisonBytes = new byte[cipher.GetOutputSize(plainTextBytes.Length)];
            int length = cipher.ProcessBytes(plainTextBytes, comparisonBytes, 0);

            _ = cipher.DoFinal(comparisonBytes, length);
            //                return Convert.ToBase64String(comparisonBytes);
            return Convert.ToBase64String(saltStringBytes.Concat(ivStringBytes).Concat(comparisonBytes).ToArray());
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (RNGCryptoServiceProvider rngCsp = new())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 258;
    }
}