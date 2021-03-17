using System;
using AM.E3dc.Rscp.Common;
using FluentAssertions;
using Xunit;

namespace AM.E3dc.Rscp.Crypto.Tests
{
    public class E3dcAes256CryptoProviderFixture
    {
        private readonly ICryptoProvider subject = new E3dcAes256CryptoProvider();

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetPasswordThrowsIfPasswordIsNullOrEmpty(string password)
        {
            var action = new Action(() => this.subject.SetPassword(password));

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.ParamName == "password");
        }

        [Fact]
        public void CanEncryptAndDecrypt()
        {
            /*
             * This ciphers used in this test case were constructed using
             * the full-blown .NET version of BouncyCastle.
             * The purpose of this test is to validate that the IVs are
             * handled according to E3/DC's specification.
             */

            this.subject.SetPassword("abc123");

            var plainTextBytes = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            var cipher1 = this.subject.Encrypt(plainTextBytes);
            cipher1.Should()
                .BeEquivalentTo(new byte[] { 0x63, 0x1C, 0xEA, 0x5D, 0xFA, 0xD4, 0x4B, 0xF2, 0x02, 0xE9, 0x44, 0xFC, 0x53, 0x33, 0x29, 0x16, 0x6B, 0x98, 0x5A, 0x3F, 0xFB, 0xC9, 0x2D, 0x02, 0x2E, 0x10, 0x01, 0x9D, 0xBE, 0xC2, 0x31, 0x7F });
            var plain1 = this.subject.Decrypt(cipher1);
            plain1.Should()
                .StartWith(plainTextBytes);

            var cipher2 = this.subject.Encrypt(plainTextBytes);
            cipher2.Should()
                .BeEquivalentTo(new byte[] { 0x65, 0x94, 0x4A, 0xBA, 0x5F, 0xD1, 0xBC, 0x61, 0x7D, 0x28, 0xE4, 0x83, 0x4E, 0x0E, 0x3F, 0x13, 0x10, 0x61, 0xEB, 0xFB, 0xCF, 0xD4, 0xC5, 0x98, 0x28, 0x7D, 0xC2, 0x97, 0x7C, 0xD1, 0xE1, 0x4E });
            var plain2 = this.subject.Decrypt(cipher2);
            plain2.Should()
                .StartWith(plainTextBytes);
        }

        [Fact]
        public void EncryptThrowsExceptionIfPlainTextIsNull()
        {
            this.subject.SetPassword("abc123");
            var action = new Action(() => this.subject.Encrypt(null));

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.ParamName == "plainTextBytes");
        }

        [Fact]
        public void EncryptThrowsExceptionIfPlainTextIsEmpty()
        {
            this.subject.SetPassword("abc123");
            var action = new Action(() => this.subject.Encrypt(new byte[0]));

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("The data to be encrypted must not be empty!");
        }

        [Fact]
        public void EncryptThrowsExceptionIfNoKeyIsSet()
        {
            var action = new Action(() => this.subject.Encrypt(new byte[] { 0x01, 0x02, 0x03 }));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot encrypt data without a key. Call SetPassword(password) to create a key.");
        }

        [Fact]
        public void DecryptThrowsExceptionIfCipherTextIsNull()
        {
            this.subject.SetPassword("abc123");
            var action = new Action(() => this.subject.Decrypt(null));

            action.Should()
                .Throw<ArgumentNullException>()
                .Where(e => e.ParamName == "cipherTextBytes");
        }

        [Fact]
        public void DecryptThrowsExceptionIfCipherTextIsEmpty()
        {
            this.subject.SetPassword("abc123");
            var action = new Action(() => this.subject.Decrypt(new byte[0]));

            action.Should()
                .Throw<ArgumentException>()
                .WithMessage("The data to be decrypted must not be empty!");
        }

        [Fact]
        public void DecryptThrowsExceptionIfNoKeyIsSet()
        {
            var action = new Action(() => this.subject.Decrypt(new byte[] { 0x01, 0x02, 0x03 }));

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Cannot decrypt data without a key. Call SetPassword(password) to create a key.");
        }
    }
}
