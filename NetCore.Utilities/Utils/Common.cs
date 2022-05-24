using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Utilities.Utils
{
    public static class Common
    {
        /// <summary>
        /// Data Protection 지정하기
        /// </summary>
        /// <param name="services">등록할 서비스</param>
        /// <param name="keyPath">키 경로</param>
        /// <param name="applicationName">애플리케이션 이름</param>
        /// <param name="cryptoType">암호화 유형</param>
        //void라는 리턴형태가 없는 메서드를 추가 13.
        //Startup.cs에서 IServiceCollection services 복사 > 누겟 2개 추가, 경로와 애플리케이션 이름 파라미터 추가/ , 이넘형태 데이터
        public static void SetDataProtection(IServiceCollection services, string keyPath, string applicationName, Enum cryptoType)
        {
            //13. DataProtection
            var builder = services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                .SetApplicationName(applicationName);//Enum으로 받아서

            //switch문으로 cryptoType을 받아서 처리
            switch (cryptoType)
            {
                case Enums.CryptoType.Unmanaged:
                //AES : Advenced Encryption Standard
                //Two-way방식 : 암호화, 복호화
                    builder.UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration()
                        {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                        });
                    break;
                case Enums.CryptoType.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration()
                        {
                            // A type that subclasses SymmetricAlgorithm
                            EncryptionAlgorithmType = typeof(Aes),

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // A type that subclasses KeyedHashAlgorithm
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        });
                    break;
                case Enums.CryptoType.CngCbc:
                    //Windows CNG algorithm using CBC-mode encryption
                    //CNG algorithm
                    //Cryptography API : Next Generation
                    //CBC-mode : 암호화 유형
                    builder.UseCustomCryptographicAlgorithms(
                        new CngCbcAuthenticatedEncryptorConfiguration()
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // Passed to BCryptOpenAlgorithmProvider
                            HashAlgorithm = "SHA512",
                            HashAlgorithmProvider = null
                        });
                    break;
                case Enums.CryptoType.CngGcm:
                    //Windows CNG algorithm using Galois/Counter Mode encryption
                    //Galois/Counter Mode
                    //GCM
                    builder.UseCustomCryptographicAlgorithms(
                         new CngGcmAuthenticatedEncryptorConfiguration()
                         {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                             EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256
                         });
                    break;

            }
        }
    }
}
