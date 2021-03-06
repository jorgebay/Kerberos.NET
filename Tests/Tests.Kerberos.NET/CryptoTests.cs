using Kerberos.NET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kerberos.NET.Entities;

namespace Tests.Kerberos.NET
{
    [TestClass]
    public class CryptoTests
    {
        [TestMethod]
        public async Task TestRC4Kerberos()
        {
            var data = File.ReadAllBytes("data\\rc4-kerberos-data");
            var key = File.ReadAllBytes("data\\rc4-key-data");

            await TestDecode(data, key, EncryptionType.RC4_HMAC_NT);
        }

        [TestMethod]
        public async Task TestRC4SPNego()
        {
            var data = File.ReadAllBytes("data\\rc4-spnego-data");
            var key = File.ReadAllBytes("data\\rc4-key-data");

            await TestDecode(data, key, EncryptionType.RC4_HMAC_NT);
        }

        [TestMethod]
        public async Task TestAES128Kerberos()
        {
            var data = File.ReadAllBytes("data\\aes128-kerberos-data");
            var key = File.ReadAllBytes("data\\aes128-key-data");

            await TestDecode(data, key, EncryptionType.AES128_CTS_HMAC_SHA1_96);
        }

        [TestMethod]
        public async Task TestAES128SPNego()
        {
            var data = File.ReadAllBytes("data\\aes128-spnego-data");
            var key = File.ReadAllBytes("data\\aes128-key-data");

            await TestDecode(data, key, EncryptionType.AES128_CTS_HMAC_SHA1_96);
        }

        [TestMethod]
        public async Task TestAES256Kerberos()
        {
            var data = File.ReadAllBytes("data\\aes256-kerberos-data");
            var key = File.ReadAllBytes("data\\aes256-key-data");

            await TestDecode(data, key, EncryptionType.AES256_CTS_HMAC_SHA1_96);
        }

        [TestMethod]
        public async Task TestAES256SPNego()
        {
            var data = File.ReadAllBytes("data\\aes256-spnego-data");
            var key = File.ReadAllBytes("data\\aes256-key-data");

            await TestDecode(data, key, EncryptionType.AES256_CTS_HMAC_SHA1_96);
        }

        private static async Task TestDecode(byte[] data, byte[] key, EncryptionType etype)
        {
            var validator = new IntrospectiveValidator(key) { ValidateAfterDecrypt = ValidationActions.Replay };

            var authenticator = new KerberosAuthenticator(validator);

            var result = await authenticator.Authenticate(data);

            Assert.IsNotNull(result);

            Assert.IsTrue(result.Claims.Count() > 0);

            Assert.AreEqual("Kerberos", result.AuthenticationType);

            Assert.AreEqual("user.test@domain.com", result.Name);

            Assert.IsNotNull(validator.Data);

            Assert.AreEqual(etype, validator.Data.EType);
        }

        private class IntrospectiveValidator : KerberosValidator
        {
            public IntrospectiveValidator(byte[] key) : base(key, ticketCache: null)
            {
            }

            public DecryptedData Data { get; set; }

            protected override Task Validate(DecryptedData decryptedToken)
            {
                Data = decryptedToken;

                return base.Validate(decryptedToken);
            }
        }
    }
}
