using DotNetInsights.Shared.Contracts;
using DotNetInsights.Shared.Contracts.Factories;
using DotNetInsights.Shared.Contracts.Services;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotNetInsights.Shared.Domains.Enumerations;

namespace DotNetInsights.Shared.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IMemoryStreamManager memoryStreamManager;
        private readonly ISymmetricAlgorithmFactory symmetricAlgorithmFactory;

        public byte[] GenerateBytes(string key, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.ASCII;

            return encoding.GetBytes(key);
        }

        public async Task<byte[]> EncryptString(SymmetricAlgorithmType symmetricAlgorithmType, string plainText, byte[] key, byte[] iV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length == 0)
                throw new ArgumentNullException(nameof(key));
            if (iV == null || iV.Length == 0)
                throw new ArgumentNullException(nameof(iV));

            byte[] encrypted = Array.Empty<byte>();

            await InvokeSymmetricAlgorithmAsync(symmetricAlgorithmType, async (symmetricAlgorithm) =>
            {
                symmetricAlgorithm.Key = key;
                symmetricAlgorithm.IV = iV;

                // Create an encryptor to perform the stream transform.
                using (var encryptor = symmetricAlgorithm.CreateEncryptor(symmetricAlgorithm.Key, symmetricAlgorithm.IV))
                using (var msEncrypt = memoryStreamManager.GetStream(false))
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        await swEncrypt.WriteAsync(plainText).ConfigureAwait(false);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }).ConfigureAwait(false);

            return encrypted;
        }

        public async Task<string> DecryptBytes(SymmetricAlgorithmType symmetricAlgorithmType, byte[] bytes, byte[] key, byte[] iV)
        {
            if (bytes == null || bytes.Length <= 0)
                throw new ArgumentNullException(nameof(bytes));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iV == null || iV.Length <= 0)
                throw new ArgumentNullException(nameof(key));

            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.

            await InvokeSymmetricAlgorithmAsync(symmetricAlgorithmType, async (symmetricAlgorithm) =>
            {
                symmetricAlgorithm.Key = key;
                symmetricAlgorithm.IV = iV;

                // Create a decryptor to perform the stream transform.
                using (var decryptor = symmetricAlgorithm.CreateDecryptor(symmetricAlgorithm.Key, symmetricAlgorithm.IV))
                // Create the streams used for decryption.
                using (var msDecrypt = memoryStreamManager.GetStream(bytes, false))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = await srDecrypt.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }
                }
            }).ConfigureAwait(false);

            return plaintext;
        }

        private SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricAlgorithmType symmetricAlgorithmType)
        {
            return symmetricAlgorithmFactory.GetSymmetricAlgorithm(symmetricAlgorithmType);
        }

        private async Task InvokeSymmetricAlgorithmAsync(SymmetricAlgorithmType symmetricAlgorithmType, Func<SymmetricAlgorithm, Task> invokeAsync)
        {
            using (var symmetricAlgorithm = GetSymmetricAlgorithm(symmetricAlgorithmType))
            {
                await invokeAsync(symmetricAlgorithm).ConfigureAwait(false);
            }
        }

        private void InvokeSymmetricAlgorithm(SymmetricAlgorithmType symmetricAlgorithmType, Action<SymmetricAlgorithm> invoke)
        {
            using (var symmetricAlgorithm = GetSymmetricAlgorithm(symmetricAlgorithmType))
            {
                invoke(symmetricAlgorithm);
            }
        }

        public byte[] GenerateIv(SymmetricAlgorithmType symmetricAlgorithmType)
        {
            var iv = Array.Empty<byte>();
            InvokeSymmetricAlgorithm(symmetricAlgorithmType, invoke: symmetricAlgorithm =>
            {
                symmetricAlgorithm.GenerateIV();
                iv = symmetricAlgorithm.IV;
            });

            return iv;
        }

        public byte[] GenerateKey(SymmetricAlgorithmType symmetricAlgorithmType)
        {
            var key = Array.Empty<byte>();
            InvokeSymmetricAlgorithm(symmetricAlgorithmType, invoke: symmetricAlgorithm =>
            {
                symmetricAlgorithm.GenerateKey();
                key = symmetricAlgorithm.Key;
            });

            return key;
        }

        public EncryptionService(IMemoryStreamManager memoryStreamManager, ISymmetricAlgorithmFactory symmetricAlgorithmFactory)
        {
            this.memoryStreamManager = memoryStreamManager;
            this.symmetricAlgorithmFactory = symmetricAlgorithmFactory;
        }
    }
}
