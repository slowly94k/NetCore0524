using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Bridges;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Svcs
{
    public class PasswordHasher : IPasswordHasher
    {
        //데이터베이스에서 불러올려면 의존성주입 방법으로 참조연결~ 16.
        private DBFirstDbContext _context;

        //생성자
        public PasswordHasher(DBFirstDbContext context)
        {
            _context = context;
        }
        
        #region private methods
        //guidSolt 메서드 생성
        private string GetGUIDSalt()
        {
            return Guid.NewGuid().ToString();
        }

        //솔트 메서드 생성
        private string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            //salt가 만들어 질 때 “RandomNumberGenerator”난수 생성기에 의해 임의의 값으로 생성된다.
            //   ⇒이름을 rngSalt로 변경하고 Base64형태로 바꾼값을 데이터베이스에 넣을 거니까 밑 처럼 작성
            return Convert.ToBase64String(salt);
        }

        //아이디와 비밀번호에 대해서 대소문자 처리 17.  ToLower()
        //해쉬 메서드 생성
        //userId, password, rngSalt 값을 받아야 해서 파라미터로 사용
        private string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            //Pbkdf2 ??
            //Password-base key derivation function 2   비밀번호를 암호화 하는데 사용하는 함수!
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId.ToLower() + password.ToLower() + guidSalt, //사용자가 원하는 값을 암호화 시킨다
                salt: Encoding.UTF8.GetBytes(rngSalt),//byte배열로 컨버팅,   salt : password를 유추할수 없도록 하기위해
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 45000, //10000, 20000, 45000 번 돌린다
                numBytesRequested: 256 / 8));//길이
        }
        //체크하는 함수
        private bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            //GetPasswordHash로 입력된 값과 passwordHash로 만들어진 값을 비교 일치 T, 불일치 F
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
        }

        //사용자 가입 서비스 UserService.cs에서~ 17.
        private PasswordHashInfo PasswordInfo(string userId, string password)
        {
            string guidSalt = GetGUIDSalt();
            string rngSalt = GetRNGSalt();

            var passwordInfo = new PasswordHashInfo()
            {
                GUIDSalt = guidSalt,
                RNGSalt = rngSalt,
                PasswordHash = GetPasswordHash(userId, password, guidSalt, rngSalt)
            };
            return passwordInfo;
        }
        #endregion

        //명시적~
        string IPasswordHasher.GetGUIDSalt()
        {
            return GetGUIDSalt();
        }

        string IPasswordHasher.GetRNGSalt()
        {
            return GetRNGSalt();
        }

        string IPasswordHasher.GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt);
        }
        
        bool IPasswordHasher.CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            return CheckThePasswordInfo(userId, password, guidSalt, rngSalt, passwordHash);
        }

        PasswordHashInfo IPasswordHasher.SetPasswordInfo(string userId, string password)
        {
            return PasswordInfo(userId, password);
        }
    }
}
