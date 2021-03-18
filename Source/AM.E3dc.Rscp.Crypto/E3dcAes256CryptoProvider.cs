using System;
using System.Linq;
using System.Text;
using AM.E3dc.Rscp.Abstractions;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace AM.E3dc.Rscp.Crypto
{
    /// <summary>
    /// This class provides encryption and decryption services according to
    /// E3/DC's specification.
    /// </summary>
    public class E3dcAes256CryptoProvider : ICryptoProvider
    {
        private readonly byte[] ivEncryption = Enumerable.Repeat((byte)0xff, 32).ToArray();
        private readonly byte[] ivDecryption = Enumerable.Repeat((byte)0xff, 32).ToArray();
        private byte[] key;

        /// <inheritdoc cref="ICryptoProvider"/>
        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            var keyBytes = Enumerable.Repeat((byte)0xff, 32).ToArray();
            Encoding.ASCII.GetBytes(password).CopyTo(keyBytes, 0);
            this.key = keyBytes;
        }

        /// <inheritdoc cref="ICryptoProvider"/>
        public byte[] Encrypt(byte[] plainTextBytes)
        {
            if (this.key == null)
            {
                throw new InvalidOperationException("Cannot encrypt data without a key. Call SetPassword(password) to create a key.");
            }

            if (plainTextBytes == null)
            {
                throw new ArgumentNullException(nameof(plainTextBytes));
            }

            if (plainTextBytes.Length == 0)
            {
                throw new ArgumentException("The data to be encrypted must not be empty!");
            }

            lock (this.ivEncryption)
            {
                var engine = new RijndaelEngine(256);
                var blockCipher = new CbcBlockCipher(engine);
                var cipher = new PaddedBufferedBlockCipher(blockCipher, new ZeroBytePadding());
                var keyParam = new KeyParameter(this.key);
                var keyParamWithIv = new ParametersWithIV(keyParam, this.ivEncryption, 0, 32);

                cipher.Init(true, keyParamWithIv);
                var cipherTextBytes = new byte[cipher.GetOutputSize(plainTextBytes.Length)];
                var length = cipher.ProcessBytes(plainTextBytes.ToArray(), cipherTextBytes, 0);

                cipher.DoFinal(cipherTextBytes, length);

                Array.Copy(cipherTextBytes, cipherTextBytes.Length - this.ivEncryption.Length, this.ivEncryption, 0, this.ivEncryption.Length);

                return cipherTextBytes;
            }
        }

        /// <inheritdoc cref="ICryptoProvider"/>
        public byte[] Decrypt(byte[] cipherTextBytes)
        {
            if (this.key == null)
            {
                throw new InvalidOperationException("Cannot decrypt data without a key. Call SetPassword(password) to create a key.");
            }

            if (cipherTextBytes == null)
            {
                throw new ArgumentNullException(nameof(cipherTextBytes));
            }

            if (cipherTextBytes.Length == 0)
            {
                throw new ArgumentException("The data to be decrypted must not be empty!");
            }

            lock (this.ivDecryption)
            {
                var engine = new RijndaelEngine(256);
                var blockCipher = new CbcBlockCipher(engine);
                var cipher = new PaddedBufferedBlockCipher(blockCipher, new ZeroBytePadding());
                var keyParam = new KeyParameter(this.key);
                var keyParamWithIv = new ParametersWithIV(keyParam, this.ivDecryption, 0, 32);

                cipher.Init(false, keyParamWithIv);
                var plainTextBytes = new byte[cipher.GetOutputSize(cipherTextBytes.Length)];
                var length = cipher.ProcessBytes(cipherTextBytes, plainTextBytes, 0);

                cipher.DoFinal(plainTextBytes, length);

                Array.Copy(cipherTextBytes, cipherTextBytes.Length - this.ivDecryption.Length, this.ivDecryption, 0, this.ivDecryption.Length);

                return plainTextBytes;
            }
        }
    }
}
