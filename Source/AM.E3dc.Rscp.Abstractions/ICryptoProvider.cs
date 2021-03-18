using System;

namespace AM.E3dc.Rscp.Abstractions
{
    /// <summary>
    /// Interface description for crypto service providers.
    /// </summary>
    public interface ICryptoProvider
    {
        /// <summary>
        /// Sets the password that is to be used for encryption and decryption.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <exception cref="ArgumentNullException">Thrown if password is null.</exception>
        /// <exception cref="ArgumentException">Thrown if password empty.</exception>
        void SetPassword(string password);

        /// <summary>
        /// Encrypts the plaintext bytes using the pre-configured password.
        /// </summary>
        /// <param name="plainTextBytes">The plain text bytes to be encrypted.</param>
        /// <returns>The encrypted input bytes.</returns>
        /// <exception cref="ArgumentNullException">Thrown if plain text bytes are null.</exception>
        /// <exception cref="ArgumentException">Thrown if plain text bytes were empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no password was configured.</exception>
        byte[] Encrypt(byte[] plainTextBytes);

        /// <summary>
        /// Decrypts the cipher text bytes using the pre-configured password.
        /// </summary>
        /// <param name="cipherTextBytes">The cipher text bytes to be decrypted.</param>
        /// <returns>The decrypted input bytes.</returns>
        /// <remarks>
        /// The decrypted bytes come with trailing zeroes as this encryption provider
        /// cannot know the message length.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if cipher text bytes are null.</exception>
        /// <exception cref="ArgumentException">Thrown if cipher text bytes were empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no password was configured.</exception>
        byte[] Decrypt(byte[] cipherTextBytes);
    }
}
