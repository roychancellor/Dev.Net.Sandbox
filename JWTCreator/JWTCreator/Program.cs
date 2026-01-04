using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.OpenSsl;

namespace JWTCreator
{
    internal class Program
    {
        static KeyType _keyType = KeyType.RSAPRIVATEKEY; // THIS WORKS WITH INTRADO TEST SYSTEM

        static void Main(string[] args)
        {
            var privateFile = _keyType == KeyType.PRIVATEKEY ? @"C:\dev.net\privateKey2048.pem" : @"";
            var publicFile = _keyType == KeyType.PRIVATEKEY ? @"C:\dev.net\publicKey2048.pem" : @"";
            string privateKey = File.ReadAllText(privateFile);
            string publicKey = File.ReadAllText(publicFile);

            var payload = MakePayload();
            var token = CreateToken(payload, privateKey);
            Console.WriteLine($"The token is:\n{token}");
            
            var payloadDecoded = DecodeToken(token, publicKey);
            Console.WriteLine($"\nThe decoded token is:\n{payloadDecoded}");

            Console.WriteLine("\nPress any key to end...");
            Console.ReadKey();
        }

        public static Dictionary<string, object> MakePayload()
        {
            var iat = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            var exp = iat + 3600;
            var toReturn = new Dictionary<string, object>
                {
                    { "iss", "12345" },
                    { "iat", iat },
                    { "exp", exp },
                    { "authorities", new string[]{ "ROLE_SEARCH" } },
                    { "orgId", "12345" },
                    { "user_name", "testscript" },
                };
            return toReturn;
        }

        public static string CreateToken(Dictionary<string, object> payload, string privateRsaKey)
        {
            RSAParameters rsaParams = new RSAParameters();
            using (var tr = new StringReader(privateRsaKey))
            {
                var pemReader = new PemReader(tr);

                if (_keyType == KeyType.RSAPRIVATEKEY)
                {
                    var keyPair = pemReader.ReadObject() as AsymmetricCipherKeyPair;
                    if (keyPair == null)
                    {
                        throw new Exception("Could not read RSA private key");
                    }
                    var privateRsaParams = keyPair.Private as RsaPrivateCrtKeyParameters;
                    rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
                }
                if (_keyType == KeyType.PRIVATEKEY)
                {
                    var keyPair = pemReader.ReadObject() as RsaPrivateCrtKeyParameters;
                    if (keyPair == null)
                    {
                        throw new Exception("Could not read RSA private key");
                    }
                    var privateRsaParams = keyPair;
                    rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);
                }
            }
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);
            }
        }

        public static string DecodeToken(string token, string publicRsaKey)
        {
            RSAParameters rsaParams = new RSAParameters();

            using (var tr = new StringReader(publicRsaKey))
            {
                var pemReader = new PemReader(tr);
                var publicKeyParams = pemReader.ReadObject() as RsaKeyParameters;
                if (publicKeyParams == null)
                {
                    throw new Exception("Could not read RSA public key");
                }
                rsaParams = DotNetUtilities.ToRSAParameters(publicKeyParams); 
            }
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                // This will throw if the signature is invalid
                return Jose.JWT.Decode(token, rsa, Jose.JwsAlgorithm.RS256);
            }
        }
    }

    public enum KeyType
    {
        PRIVATEKEY,
        RSAPRIVATEKEY,
    }
}
