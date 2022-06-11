/*
 * This work (Modern Encryption of a String C#, by James Tuley), 
 * identified by James Tuley, is free of known copyright restrictions.
 * https://gist.github.com/4336842
 * http://creativecommons.org/publicdomain/mark/1.0/ 
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ebill.Utils
{
    public static class AESThenHMAC
    {
        private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

        //Preconfigured Encryption Parameters
        public static readonly int BlockBitSize = 128;
        public static readonly int KeyBitSize = 128;

        //Preconfigured Password Key Derivation Parameters
        public static readonly int SaltBitSize = 64;
        public static readonly int Iterations = 10000;
        public static readonly int MinPasswordLength = 12;

        /// <summary>
        /// Helper that generates a random key on each call.
        /// </summary>
        /// <returns></returns>
        public static byte[] NewKey()
        {
            var key = new byte[KeyBitSize / 8];
            Random.GetBytes(key);
            return key;
        }

        /// <summary>
        /// Simple Encryption (AES) then Authentication (HMAC) for a UTF8 Message.
        /// </summary>
        /// <param name="secretMessage">The secret message.</param>
        /// <param name="cryptKey">The crypt key.</param>
        /// <param name="authKey">The auth key.</param>
        /// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
        /// <returns>
        /// Encrypted Message
        /// </returns>
        /// <exception cref="System.ArgumentException">Secret Message Required!;secretMessage</exception>
        /// <remarks>
        /// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +  HMac-Tag(32)) * 1.33 Base64
        /// </remarks>
        public static string SimpleEncrypt(string secretMessage, byte[] cryptKey, byte[] authKey,
        byte[] iv,
                           byte[] nonSecretPayload = null)
        {
            if (string.IsNullOrEmpty(secretMessage))
                throw new ArgumentException("Secret Message Required!", "secretMessage");

            var plainText = Encoding.UTF8.GetBytes(secretMessage);
            var cipherText = SimpleEncrypt(plainText, cryptKey, authKey, iv, nonSecretPayload);
            
            return Convert.ToBase64String(cipherText);
        }

        /// <summary>
        /// Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message.</param>
        /// <param name="cryptKey">The crypt key.</param>
        /// <param name="authKey">The auth key.</param>
        /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
        /// <returns>
        /// Decrypted Message
        /// </returns>
        /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
        public static string SimpleDecrypt(string encryptedMessage, byte[] cryptKey, byte[] authKey,
                           int nonSecretPayloadLength = 0)
        {
            if (string.IsNullOrWhiteSpace(encryptedMessage))
                throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

            var cipherText = Convert.FromBase64String(encryptedMessage);
            var plainText = SimpleDecrypt(cipherText, cryptKey, authKey, nonSecretPayloadLength);

            return plainText == null ? null : Encoding.UTF8.GetString(plainText);
        }

        /// <summary>
        /// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message
        /// using Keys derived from a Password (PBKDF2).
        /// </summary>
        /// <param name="secretMessage">The secret message.</param>
        /// <param name="password">The password.</param>
        /// <param name="nonSecretPayload">The non secret payload.</param>
        /// <returns>
        /// Encrypted Message
        /// </returns>
        /// <exception cref="System.ArgumentException">password</exception>
        /// <remarks>
        /// Significantly less secure than using random binary keys.
        /// Adds additional non secret payload for key generation parameters.
        /// </remarks>
        public static string SimpleEncryptWithPassword(string secretMessage, string password,
                                 byte[] nonSecretPayload = null)
        {
            if (string.IsNullOrEmpty(secretMessage))
                throw new ArgumentException("Secret Message Required!", "secretMessage");

            var plainText = Encoding.UTF8.GetBytes(secretMessage);
            var cipherText = SimpleEncryptWithPassword(plainText, password, nonSecretPayload);
            return Convert.ToBase64String(cipherText);
        }

        /// <summary>
        /// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message
        /// using keys derived from a password (PBKDF2). 
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message.</param>
        /// <param name="password">The password.</param>
        /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
        /// <returns>
        /// Decrypted Message
        /// </returns>
        /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
        /// <remarks>
        /// Significantly less secure than using random binary keys.
        /// </remarks>
        public static string SimpleDecryptWithPassword(string encryptedMessage, string password,
                                 int nonSecretPayloadLength = 0)
        {
            if (string.IsNullOrWhiteSpace(encryptedMessage))
                throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

            var cipherText = Convert.FromBase64String(encryptedMessage);
            var plainText = SimpleDecryptWithPassword(cipherText, password, nonSecretPayloadLength);
            return plainText == null ? null : Encoding.UTF8.GetString(plainText);
        }

        public static byte[] GetIV()
        {
            byte[] cipherText;
            byte[] iv;

            using (var aes = new AesManaged
            {
                KeySize = KeyBitSize,
                BlockSize = BlockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                //Use random IV
                aes.GenerateIV();
                iv = aes.IV;
            }

            return iv;
        }

        public static byte[] SimpleEncrypt(byte[] secretMessage, byte[] cryptKey, byte[] authKey, byte[] iv, byte[] nonSecretPayload = null)
        {
            //User Error Checks
            if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "cryptKey");

            if (authKey == null || authKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "authKey");

            if (secretMessage == null || secretMessage.Length < 1)
                throw new ArgumentException("Secret Message Required!", "secretMessage");

            //non-secret payload optional
            nonSecretPayload = nonSecretPayload ?? new byte[] { };

            byte[] cipherText;
            //byte[] iv;

            using (var aes = new AesManaged
            {
                KeySize = KeyBitSize,
                BlockSize = BlockBitSize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            {
                //Use random IV
                // aes.GenerateIV();
                // iv = aes.IV;

                using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
                using (var cipherStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        //Encrypt Data
                        binaryWriter.Write(secretMessage);
                    }

                    cipherText = cipherStream.ToArray();
                }

            }

            // using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
            // using (var cipherStream = new MemoryStream())
            // {
            //     using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
            //     using (var binaryWriter = new BinaryWriter(cryptoStream))
            //     {
            //         //Encrypt Data
            //         binaryWriter.Write(secretMessage);
            //     }

            //     cipherText = cipherStream.ToArray();
            // }

            //Assemble encrypted message and add authentication
            using (var hmac = new HMACSHA256(authKey))
            using (var encryptedStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(encryptedStream))
                {
                    //Prepend non-secret payload if any
                    binaryWriter.Write(nonSecretPayload);
                    //Prepend IV
                    binaryWriter.Write(iv);
                    //Write Ciphertext
                    binaryWriter.Write(cipherText);
                    binaryWriter.Flush();

                    //Authenticate all data
                    var tag = hmac.ComputeHash(encryptedStream.ToArray());
                    //Postpend tag
                    binaryWriter.Write(tag);
                }
                return encryptedStream.ToArray();
            }

        }

        public static byte[] SimpleDecrypt(byte[] encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0)
        {

            //Basic Usage Error Checks
            if (cryptKey == null || cryptKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("CryptKey needs to be {0} bit!", KeyBitSize), "cryptKey");

            if (authKey == null || authKey.Length != KeyBitSize / 8)
                throw new ArgumentException(String.Format("AuthKey needs to be {0} bit!", KeyBitSize), "authKey");

            if (encryptedMessage == null || encryptedMessage.Length == 0)
                throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

            using (var hmac = new HMACSHA256(authKey))
            {
                var sentTag = new byte[hmac.HashSize / 8];
                //Calculate Tag
                var calcTag = hmac.ComputeHash(encryptedMessage, 0, encryptedMessage.Length - sentTag.Length);
                var ivLength = (BlockBitSize / 8);

                //if message length is to small just return null
                if (encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength)
                    return null;

                //Grab Sent Tag
                Array.Copy(encryptedMessage, encryptedMessage.Length - sentTag.Length, sentTag, 0, sentTag.Length);

                //Compare Tag with constant time comparison
                var compare = 0;
                for (var i = 0; i < sentTag.Length; i++)
                    compare |= sentTag[i] ^ calcTag[i];

                //if message doesn't authenticate return null
                if (compare != 0)
                    return null;

                using (var aes = new AesManaged
                {
                    KeySize = KeyBitSize,
                    BlockSize = BlockBitSize,
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                })
                {

                    //Grab IV from message
                    var iv = new byte[ivLength];
                    Array.Copy(encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length);

                    using (var decrypter = aes.CreateDecryptor(cryptKey, iv))
                    using (var plainTextStream = new MemoryStream())
                    {
                        using (var decrypterStream = new CryptoStream(plainTextStream, decrypter, CryptoStreamMode.Write))
                        using (var binaryWriter = new BinaryWriter(decrypterStream))
                        {
                            //Decrypt Cipher Text from Message
                            binaryWriter.Write(
                              encryptedMessage,
                              nonSecretPayloadLength + iv.Length,
                              encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length
                            );
                        }
                        //Return Plain Text
                        return plainTextStream.ToArray();
                    }
                }
            }
        }

        public static byte[] SimpleEncryptWithPassword(byte[] secretMessage, string password, byte[] nonSecretPayload = null)
        {
            nonSecretPayload = nonSecretPayload ?? new byte[] { };

            //User Error Checks
            if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
                throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

            if (secretMessage == null || secretMessage.Length == 0)
                throw new ArgumentException("Secret Message Required!", "secretMessage");

            var payload = new byte[((SaltBitSize / 8) * 2) + nonSecretPayload.Length];

            Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
            int payloadIndex = nonSecretPayload.Length;

            byte[] cryptKey;
            byte[] authKey;
            //Use Random Salt to prevent pre-generated weak password attacks.
            using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize / 8, Iterations))
            {
                var salt = generator.Salt;

                //Generate Keys
                cryptKey = generator.GetBytes(KeyBitSize / 8);

                //Create Non Secret Payload
                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
                payloadIndex += salt.Length;
            }

            //Deriving separate key, might be less efficient than using HKDF, 
            //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
            using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize / 8, Iterations))
            {
                var salt = generator.Salt;

                //Generate Keys
                authKey = generator.GetBytes(KeyBitSize / 8);

                //Create Rest of Non Secret Payload
                Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
            }

            return SimpleEncrypt(secretMessage, cryptKey, authKey, payload);
        }

        public static byte[] SimpleDecryptWithPassword(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0)
        {
            //User Error Checks
            if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
                throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

            if (encryptedMessage == null || encryptedMessage.Length == 0)
                throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

            var cryptSalt = new byte[SaltBitSize / 8];
            var authSalt = new byte[SaltBitSize / 8];

            //Grab Salt from Non-Secret Payload
            Array.Copy(encryptedMessage, nonSecretPayloadLength, cryptSalt, 0, cryptSalt.Length);
            Array.Copy(encryptedMessage, nonSecretPayloadLength + cryptSalt.Length, authSalt, 0, authSalt.Length);

            byte[] cryptKey;
            byte[] authKey;

            //Generate crypt key
            using (var generator = new Rfc2898DeriveBytes(password, cryptSalt, Iterations))
            {
                cryptKey = generator.GetBytes(KeyBitSize / 8);
            }
            //Generate auth key
            using (var generator = new Rfc2898DeriveBytes(password, authSalt, Iterations))
            {
                authKey = generator.GetBytes(KeyBitSize / 8);
            }

            return SimpleDecrypt(encryptedMessage, cryptKey, authKey, cryptSalt.Length + authSalt.Length + nonSecretPayloadLength);
        }
    }
}

// //Bouncy Castle AES-GCM [Gist]

// /*
//  * This work (Modern Encryption of a String C#, by James Tuley), 
//  * identified by James Tuley, is free of known copyright restrictions.
//  * https://gist.github.com/4336842
//  * http://creativecommons.org/publicdomain/mark/1.0/ 
//  */

// using System;
// using System.IO;
// using System.Text;
// using Org.BouncyCastle.Crypto;
// using Org.BouncyCastle.Crypto.Engines;
// using Org.BouncyCastle.Crypto.Generators;
// using Org.BouncyCastle.Crypto.Modes;
// using Org.BouncyCastle.Crypto.Parameters;
// using Org.BouncyCastle.Security;
// namespace Encryption
// {

//   public static class AESGCM
//   {
//     private static readonly SecureRandom Random = new SecureRandom();

//     //Preconfigured Encryption Parameters
//     public static readonly int NonceBitSize = 128;
//     public static readonly int MacBitSize = 128;
//     public static readonly int KeyBitSize = 256;

//     //Preconfigured Password Key Derivation Parameters
//     public static readonly int SaltBitSize = 128;
//     public static readonly int Iterations = 10000;
//     public static readonly int MinPasswordLength = 12;


//     /// <summary>
//     /// Helper that generates a random new key on each call.
//     /// </summary>
//     /// <returns></returns>
//     public static byte[] NewKey()
//     {
//       var key = new byte[KeyBitSize / 8];
//       Random.NextBytes(key);
//       return key;
//     }

//     /// <summary>
//     /// Simple Encryption And Authentication (AES-GCM) of a UTF8 string.
//     /// </summary>
//     /// <param name="secretMessage">The secret message.</param>
//     /// <param name="key">The key.</param>
//     /// <param name="nonSecretPayload">Optional non-secret payload.</param>
//     /// <returns>
//     /// Encrypted Message
//     /// </returns>
//     /// <exception cref="System.ArgumentException">Secret Message Required!;secretMessage</exception>
//     /// <remarks>
//     /// Adds overhead of (Optional-Payload + BlockSize(16) + Message +  HMac-Tag(16)) * 1.33 Base64
//     /// </remarks>
//     public static string SimpleEncrypt(string secretMessage, byte[] key, byte[] nonSecretPayload = null)
//     {
//       if (string.IsNullOrEmpty(secretMessage))
//         throw new ArgumentException("Secret Message Required!", "secretMessage");

//       var plainText = Encoding.UTF8.GetBytes(secretMessage);
//       var cipherText = SimpleEncrypt(plainText, key, nonSecretPayload);
//       return Convert.ToBase64String(cipherText);
//     }


//     /// <summary>
//     /// Simple Decryption & Authentication (AES-GCM) of a UTF8 Message
//     /// </summary>
//     /// <param name="encryptedMessage">The encrypted message.</param>
//     /// <param name="key">The key.</param>
//     /// <param name="nonSecretPayloadLength">Length of the optional non-secret payload.</param>
//     /// <returns>Decrypted Message</returns>
//     public static string SimpleDecrypt(string encryptedMessage, byte[] key, int nonSecretPayloadLength = 0)
//     {
//       if (string.IsNullOrEmpty(encryptedMessage))
//         throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

//       var cipherText = Convert.FromBase64String(encryptedMessage);
//       var plainText = SimpleDecrypt(cipherText, key, nonSecretPayloadLength);
//       return plainText == null ? null : Encoding.UTF8.GetString(plainText);
//     }

//     /// <summary>
//     /// Simple Encryption And Authentication (AES-GCM) of a UTF8 String
//     /// using key derived from a password (PBKDF2).
//     /// </summary>
//     /// <param name="secretMessage">The secret message.</param>
//     /// <param name="password">The password.</param>
//     /// <param name="nonSecretPayload">The non secret payload.</param>
//     /// <returns>
//     /// Encrypted Message
//     /// </returns>
//     /// <remarks>
//     /// Significantly less secure than using random binary keys.
//     /// Adds additional non secret payload for key generation parameters.
//     /// </remarks>
//     public static string SimpleEncryptWithPassword(string secretMessage, string password,
//                              byte[] nonSecretPayload = null)
//     {
//       if (string.IsNullOrEmpty(secretMessage))
//         throw new ArgumentException("Secret Message Required!", "secretMessage");

//       var plainText = Encoding.UTF8.GetBytes(secretMessage);
//       var cipherText = SimpleEncryptWithPassword(plainText, password, nonSecretPayload);
//       return Convert.ToBase64String(cipherText);
//     }


//     /// <summary>
//     /// Simple Decryption and Authentication (AES-GCM) of a UTF8 message
//     /// using a key derived from a password (PBKDF2)
//     /// </summary>
//     /// <param name="encryptedMessage">The encrypted message.</param>
//     /// <param name="password">The password.</param>
//     /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
//     /// <returns>
//     /// Decrypted Message
//     /// </returns>
//     /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
//     /// <remarks>
//     /// Significantly less secure than using random binary keys.
//     /// </remarks>
//     public static string SimpleDecryptWithPassword(string encryptedMessage, string password,
//                              int nonSecretPayloadLength = 0)
//     {
//       if (string.IsNullOrWhiteSpace(encryptedMessage))
//         throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

//       var cipherText = Convert.FromBase64String(encryptedMessage);
//       var plainText = SimpleDecryptWithPassword(cipherText, password, nonSecretPayloadLength);
//       return plainText == null ? null : Encoding.UTF8.GetString(plainText);
//     }

//     public static byte[] SimpleEncrypt(byte[] secretMessage, byte[] key, byte[] nonSecretPayload = null)
//     {
//       //User Error Checks
//       if (key == null || key.Length != KeyBitSize / 8)
//         throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "key");

//       if (secretMessage == null || secretMessage.Length == 0)
//         throw new ArgumentException("Secret Message Required!", "secretMessage");

//       //Non-secret Payload Optional
//       nonSecretPayload = nonSecretPayload ?? new byte[] { };

//       //Using random nonce large enough not to repeat
//       var nonce = new byte[NonceBitSize / 8];
//       Random.NextBytes(nonce, 0, nonce.Length);

//       var cipher = new GcmBlockCipher(new AesFastEngine());
//       var parameters = new AeadParameters(new KeyParameter(key), MacBitSize, nonce, nonSecretPayload);
//       cipher.Init(true, parameters);

//       //Generate Cipher Text With Auth Tag
//       var cipherText = new byte[cipher.GetOutputSize(secretMessage.Length)];
//       var len = cipher.ProcessBytes(secretMessage, 0, secretMessage.Length, cipherText, 0);
//       cipher.DoFinal(cipherText, len);

//       //Assemble Message
//       using (var combinedStream = new MemoryStream())
//       {
//         using (var binaryWriter = new BinaryWriter(combinedStream))
//         {
//           //Prepend Authenticated Payload
//           binaryWriter.Write(nonSecretPayload);
//           //Prepend Nonce
//           binaryWriter.Write(nonce);
//           //Write Cipher Text
//           binaryWriter.Write(cipherText);
//         }
//         return combinedStream.ToArray();
//       }
//     }

//     public static byte[] SimpleDecrypt(byte[] encryptedMessage, byte[] key, int nonSecretPayloadLength = 0)
//     {
//       //User Error Checks
//       if (key == null || key.Length != KeyBitSize / 8)
//         throw new ArgumentException(String.Format("Key needs to be {0} bit!", KeyBitSize), "key");

//       if (encryptedMessage == null || encryptedMessage.Length == 0)
//         throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

//       using (var cipherStream = new MemoryStream(encryptedMessage))
//       using (var cipherReader = new BinaryReader(cipherStream))
//       {
//         //Grab Payload
//         var nonSecretPayload = cipherReader.ReadBytes(nonSecretPayloadLength);

//         //Grab Nonce
//         var nonce = cipherReader.ReadBytes(NonceBitSize / 8);

//         var cipher = new GcmBlockCipher(new AesFastEngine());
//         var parameters = new AeadParameters(new KeyParameter(key), MacBitSize, nonce, nonSecretPayload);
//         cipher.Init(false, parameters);

//         //Decrypt Cipher Text
//         var cipherText = cipherReader.ReadBytes(encryptedMessage.Length - nonSecretPayloadLength - nonce.Length);
//         var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];  

//         try
//         {
//           var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
//           cipher.DoFinal(plainText, len);

//         }
//         catch (InvalidCipherTextException)
//         {
//           //Return null if it doesn't authenticate
//           return null;
//         }

//         return plainText;
//       }

//     }

//     public static byte[] SimpleEncryptWithPassword(byte[] secretMessage, string password, byte[] nonSecretPayload = null)
//     {
//       nonSecretPayload = nonSecretPayload ?? new byte[] {};

//       //User Error Checks
//       if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
//         throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

//       if (secretMessage == null || secretMessage.Length == 0)
//         throw new ArgumentException("Secret Message Required!", "secretMessage");

//       var generator = new Pkcs5S2ParametersGenerator();

//       //Use Random Salt to minimize pre-generated weak password attacks.
//       var salt = new byte[SaltBitSize / 8];
//       Random.NextBytes(salt);

//       generator.Init(
//         PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()),
//         salt,
//         Iterations);

//       //Generate Key
//       var key = (KeyParameter)generator.GenerateDerivedMacParameters(KeyBitSize);

//       //Create Full Non Secret Payload
//       var payload = new byte[salt.Length + nonSecretPayload.Length];
//       Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
//       Array.Copy(salt,0, payload,nonSecretPayload.Length, salt.Length);

//       return SimpleEncrypt(secretMessage, key.GetKey(), payload);
//     }

//     public static byte[] SimpleDecryptWithPassword(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0)
//     {
//       //User Error Checks
//       if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
//         throw new ArgumentException(String.Format("Must have a password of at least {0} characters!", MinPasswordLength), "password");

//       if (encryptedMessage == null || encryptedMessage.Length == 0)
//         throw new ArgumentException("Encrypted Message Required!", "encryptedMessage");

//       var generator = new Pkcs5S2ParametersGenerator();

//       //Grab Salt from Payload
//       var salt = new byte[SaltBitSize / 8];
//       Array.Copy(encryptedMessage, nonSecretPayloadLength, salt, 0, salt.Length);

//       generator.Init(
//         PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()),
//         salt,
//         Iterations);

//       //Generate Key
//       var key = (KeyParameter)generator.GenerateDerivedMacParameters(KeyBitSize);

//       return SimpleDecrypt(encryptedMessage, key.GetKey(), salt.Length + nonSecretPayloadLength);
//     }
//   }
// }