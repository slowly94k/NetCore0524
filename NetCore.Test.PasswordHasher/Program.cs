using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace NetCore.Test.PasswordHasher
{
    class Program
    {
        //데이터베이스에
        //Password컬럼을 대신해서 => GUIDSalt, RNGSalt, PasswordHash(전체 결과가 있는) 추가
        static void Main(string[] args)
        {
            Console.Write("아이디를 입력하세요: ");
            string userId = Console.ReadLine();

            Console.Write("비밀번호를 입력하세요: ");
            string password = Console.ReadLine();

            //조금 더 복잡성을 추가한다
            //Guid.NewGuid() : 새로 생성되는 것. ToString으로 문자열로 바꿔주면 계속 값이 바뀐다
            string guidSalt = Guid.NewGuid().ToString();

            //rngSalt 지정
            //()안에 파라메터 쓸 필요 없다 => 내부에서 RandomNumberGenerator를 통해서 Create()생성해서 만들기 때문
            string rngSalt = GetRNGSalt();

            //passwordHash지정
            string passwordHash = GetPasswordHash(userId, password, guidSalt, rngSalt);

            //체크하는 함수
            //데이터베이스의 비밀번호정보와 지금 입력한 비밀번호정보를 비교해서 같은 해시값이 나오면 로그인 성공
            bool check = CheckThePasswordInfo(userId, password, guidSalt, rngSalt, passwordHash);

            Console.WriteLine($"UserId:{userId}");
            Console.WriteLine($"Password:{password}");
            Console.WriteLine($"GUIDSalt:{guidSalt}");
            Console.WriteLine($"RNGSalt: {rngSalt}");
            Console.WriteLine($"Hashed: {passwordHash}");
            Console.WriteLine($"check:{(check ? "비밀번호 정보가 일치" : "불일치")}");

            //콘솔창이 안닫치게 하는 것
            Console.ReadLine();
        }
        //솔트 메서드 생성
        private static string GetRNGSalt()
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

        //해쉬 메서드 생성
        //userId, password, rngSalt 값을 받아야 해서 파라미터로 사용
        private static string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            //Pbkdf2 ??
            //Password-base key derivation function 2   비밀번호를 암호화 하는데 사용하는 함수!
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId + password + guidSalt, //사용자가 원하는 값을 암호화 시킨다
                salt: Encoding.UTF8.GetBytes(rngSalt),//byte배열로 컨버팅,   salt : password를 유추할수 없도록 하기위해
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 45000, //10000, 20000, 45000 번 돌린다
                numBytesRequested: 256 / 8));//길이
        }
        //체크하는 함수
        private static bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            //GetPasswordHash로 입력된 값과 passwordHash로 만들어진 값을 비교 일치 T, 불일치 F
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
        }
    }
}
