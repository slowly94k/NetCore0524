using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
//using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {

        //의존성 주입 추가 (9.)
        private DBFirstDbContext _context;
        private IPasswordHasher _hasher;
        
        //생성자를 만들어 의존성 주입을 받아야한다 
        public UserService(DBFirstDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        //사용자 정보가 있는 테이블(User)을 가져오는 메서드 생성4.
        #region priavate method
        private IEnumerable<User> GetUserInfos()
        {
            //리스트를 가져와야하는데 _context의 DBFirstDbContext를 먼저 가져와
            //Users라는 User테이블 리스트 형태를 가져온다
            return _context.Users.ToList();
            //return new List<User>()
            //{
            //    new User()
            //    {
            //        UserId = "jadejs",
            //        UserName = "김정수",
            //        UserEmail ="jadejskim@gmail.com",
            //        Password = "123456"
            //    }
            //};
        }

        //메서드 추가 (EFCore FromSqlRaw() ),     데이터 단일건 
        //파라메타로 아이디와 비번을 직접 입력받는다. => 데이터 베이스에 있는 사용자 정보를 가져온다!
        private User GetUserInfo(string userId, string password)
        {
            User user;

            //1.Lambda 식
            //파라미터 값과 비교
            //FirstOrDefault(); 한건의 데이터만 가져온다
            //람다식을 통해서 Users테이블리스트 형태의 개체에 where절을 통해서 람다식으로 아이디와 비번을 비교해서 값을 도출
            //user = _context.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();


            //2.FromSqlRaw
            //쿼리문을 직접 입력을 해서 가져올수 있다

            //2.1Table (9.)
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate From dbo.[User]")
            //                    .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //                    .FirstOrDefault();

            //2.2VIEW (9.) Table이랑 비슷한테 From테이블쪽 [User] => uvwUser 수정
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate From dbo.uvwUser")
            //                    .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //                    .FirstOrDefault();

            //2.3FUNCTION (9.)
            //FUNCTION와 STORED PROCEDURE은 파라미터 지정가능! where절을 따로 안쓴다 
            //user = _context.Users.FromSqlInterpolated($"SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate FROM dbo.ufnUser({userId},{password})")
            //                     .FirstOrDefault();

            //2.4STORED PROCEDURE(11.)
            //FromSqlRaw()메서드 뒤에 .AsEnumerable() 메서드를 추가
            //파라메터 @p3 등 추가 가능
            //user = _context.Users.FromSqlRaw("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password })
            //                        .AsEnumerable().FirstOrDefault();
            user = _context.Users.FromSqlInterpolated($"dbo.uspCheckLoginByUserId {userId}, {password}")
                                  .AsEnumerable().FirstOrDefault();

            //사용자 정보가 없을 경우(12.)
            //비밀번호가 틀려야 이 쪽으로 넘어온다
            if (user == null)
            {
                //접속실패횟수에 대한 증가
                int rowAffected;


                //SQL문 직접 작성 12.(1. 쿼리 문으로도)
                //ExecuteSqlInterpolated는 테이블리스트(Users)를 받아오는 것이 아닌
                //Database에서 직접 연결되는 메서드!
                //rowAffected = _context.Database.ExecuteSqlInterpolated($"Update dbo.[User] SET AccessFailedCount += 1 WHERE UserId={userId}");

                //STORED PROCEDURE 12.(2. 프로시져로)
                //rowAffected = _context.Database.ExecuteSqlRaw("dbo.FailedLoginByUserId @p0", parameters: new[] { userId });
                rowAffected = _context.Database.ExecuteSqlInterpolated($"dbo.FailedLoginByUserId {userId}");
            }

            return user;

        }

        //checkTheUserInfo에서 id와 password입력받은 것을
        //GetUserInfos()에서 id와 password를 람다식으로
        //데이터가 있으면 true 없으면 false로 (9.)
        private bool checkTheUserInfo(string userId, string password)
        {
            //Any() : 리스트 형태에서만 사용가능
            //return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();
            return GetUserInfo(userId, password) != null ? true : false;
        }

        //멤버컨트롤에서 사용자 아이디 가져오는 부분 14.
        private User GetUserInfo(string userId)
        {
            return _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
        }

        //사용자권한부분 14.
        private IEnumerable<UserRolesByUser> GetUserRolesByUserInfos(string userId)
        {
            var userRolesByUserInfos = _context.UserRolesByUsers.Where(uru => uru.UserId.Equals(userId)).ToList();

            //foreach : 권한에 대한 이름과 우선순위를 가져오기위해 사용
            //사용자 소유 권한, 권한이 아이디만 있는데
            foreach (var role in userRolesByUserInfos)
            {
                //사용자 소유 권한 안에 있는 사용자 권한정보에 값이 들어간다
                role.UserRole = GetUserRole(role.RoleId);
            }
            //GetRolesOwnedByUser : 구현으로 이동 > OrderByDescending : 내림차순
            return userRolesByUserInfos.OrderByDescending(uru => uru.UserRole.RolePriority);
        }

        //UserRole 을 만들기 위해 메서드 추가 14.
        private UserRole GetUserRole(string roleId)
        {
            return _context.UserRoles.Where(ur => ur.RoleId.Equals(roleId)).FirstOrDefault();
        }

        //사용자 가입 서비스 17.
        //아이디에 대해서 대소문자 처리 
        private int RegisterUser(RegisterInfo register)
        {
            var utcNow = DateTime.UtcNow;
            //값을 가져와야한다 PasswordHasher.cs 로~
            var passwordInfo = _hasher.SetPasswordInfo(register.UserId, register.Password);

            var user = new User()
            {
                UserId = register.UserId.ToLower(),
                UserName = register.UserName,
                UserEmail = register.UserEmail,
                GUIDSalt = passwordInfo.GUIDSalt,
                RNGSalt = passwordInfo.RNGSalt,
                PasswordHash = passwordInfo.PasswordHash,
                AccessFailedCount = 0,
                IsMembershipWithdrawn = false,
                JoinedUtcDate = utcNow
            };

            //사용자 소유권한 17.
            var userRolesByUser = new UserRolesByUser()
            {
                UserId = register.UserId,
                RoleId = "AssociateUser",
                OwnedUtcDate = utcNow
            };

            _context.Add(user);
            _context.Add(userRolesByUser);

            return _context.SaveChanges();
        }

        //UpdateInfo() 액션 메서드이 Get 방식부분 18.
        //아이디를 받아와야한다.
        //회원정보 페이지에서 아이디를 자동적으로 들어가게 안하고
        //아이디와 비밀번호를 입력해서 일치할 경우만 사용자 정보 수정이 일어나게 할 것.
        private UserInfo GetUserInfoForUpdate(string userId)
        {
            var user = GetUserInfo(userId);
            var userInfo = new UserInfo()
            {
                UserId = null,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                //추가 => GetUserInfoForUpdate()안에 ChangeInfo 값을 같이 넣어준다
                ChangeInfo = new ChangeInfo()
                {
                    UserName = user.UserName,
                    UserEmail = user.UserEmail
                }
            };

            return userInfo;
        }

        //정보수정 성공 실패 여부 18.
        private int UpdateUser(UserInfo user)
        {
            //사용자 정보를 데이터베이스에서 입력 받은 값으로 비교해서 Null이면 리턴, 아니면 넘어와서 bool로 체크
            var userInfo = _context.Users.Where(u => u.UserId.Equals(user.UserId)).FirstOrDefault();

            //해당 유저 아이디에 대응되는 사용자가 없을 경우 
            if (userInfo == null)
            {
                return 0;
            }

            bool check = _hasher.CheckThePasswordInfo(user.UserId, user.Password, userInfo.GUIDSalt, userInfo.RNGSalt, userInfo.PasswordHash);

            int rowAffected = 0;

            if (check)
            {
                _context.Update(userInfo);

                userInfo.UserName = user.UserName;
                userInfo.UserEmail = user.UserEmail;

                rowAffected = _context.SaveChanges();
            }

            return rowAffected;
        }

        private bool MatchTheUserInfo(LoginInfo login)
        {
            //데이터베이스에 있는것 불러온다. 값이 하나니까 FirstOrDefault() 16. => PasswordHasher에서 여기로 추가 17.
            var user = _context.Users.Where(u => u.UserId.Equals(login.UserId)).FirstOrDefault();

            //해당 유저아이디에 대응되는 사용자가 없을 경우!! 17.
            if (user == null)
            {
                return false;
            }

            return _hasher.CheckThePasswordInfo(login.UserId, login.Password, user.GUIDSalt, user.RNGSalt, user.PasswordHash);
        }

        //변경대상 값들을 비교 18.
        private bool CompareInfo(UserInfo user)
        {
            return user.ChangeInfo.Equals(user);
        }
        

        #endregion


        //인터페이스를 상속받은 후에 명시적으로 인터페이스 구현 
        bool IUser.MatchTheUserInfo(LoginInfo login)
        {
            //return checkTheUserInfo(login.UserId, login.Password); 사용 안한 17.
            return MatchTheUserInfo(login);
           
        }

        User IUser.GetUserInfo(string userId)
        {
            return GetUserInfo(userId);
        }


        //명시적으로 구현 (14. )
        IEnumerable<UserRolesByUser> IUser.GetRolesOwnedByUser(string userId)
        {
            return GetUserRolesByUserInfos(userId);
        }

        int IUser.RegisterUser(RegisterInfo register)
        {
            return RegisterUser(register);
        }

        //명시적 18.
        UserInfo IUser.GetUserInfoForUpdate(string userId)
        {
            return GetUserInfoForUpdate(userId);
        }

        int IUser.UpdateUser(UserInfo user)
        {
            return UpdateUser(user);
        }

        bool IUser.CompareInfo(UserInfo user)
        {
            return CompareInfo(user);
        }
    }
}
