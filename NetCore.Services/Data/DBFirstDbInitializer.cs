using NetCore.Data.Classes;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Data
{
    //19.
    public class DBFirstDbInitializer
    {
        //의존성 주입
        private DBFirstDbContext _context;
        private IPasswordHasher _hasher;

        //생성자 생성 
        public DBFirstDbInitializer(DBFirstDbContext context, IPasswordHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        /// <summary>
        /// 초기 데이터를 심는다 // 이 작업 할려면 MSSQL에서 값 다 DELET한다.
        /// </summary>
        public int PlantSeedData()
        {
            int rowAffected = 0;

            string userId = "jsootv";
            string password = "123456";
            var utcNow = DateTime.UtcNow;

            var passwordInfo = _hasher.SetPasswordInfo(userId, password);

            _context.Database.EnsureCreated();
            //초기 사용자 데이터
            if (!_context.Users.Any())
            {
                var users = new List<User>()
                {
                    new User()
                    {
                        UserId = userId.ToLower(),
                        UserName = "Seed 사용자",
                        UserEmail = "jsootv@gmail.com",
                        GUIDSalt = passwordInfo.GUIDSalt,
                        RNGSalt = passwordInfo.RNGSalt,
                        PasswordHash = passwordInfo.PasswordHash,
                        AccessFailedCount = 0,
                        IsMembershipWithdrawn = false,
                        JoinedUtcDate = utcNow
                    }
                };

                _context.Users.AddRange(users);
                rowAffected += _context.SaveChanges();
            }
            //사용자 권한 데이터
            if (!_context.UserRoles.Any())
            {
                var userRoles = new List<UserRole>()
                {
                    new UserRole()
                    {
                        RoleId = "AssociateUser",
                        RoleName = "준사용자",
                        RolePriority = 1,
                        ModifiedUtcDate = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "GeneralUser",
                        RoleName = "일반사용자",
                        RolePriority = 2,
                        ModifiedUtcDate = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "SuperUser",
                        RoleName = "향상된 사용자",
                        RolePriority = 3,
                        ModifiedUtcDate = utcNow
                    },
                    new UserRole()
                    {
                        RoleId = "SystemUser",
                        RoleName = "시스템 사용자",
                        RolePriority = 4,
                        ModifiedUtcDate = utcNow
                    }
                };

                _context.UserRoles.AddRange(userRoles);
                rowAffected += _context.SaveChanges();
            }
            //사용자 소유권한 데이터
            if (!_context.UserRolesByUsers.Any())
            {
                var userRolesByUsers = new List<UserRolesByUser>()
                {
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "GeneralUser",
                        OwnedUtcDate = utcNow
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SuperUser",
                        OwnedUtcDate = utcNow
                    },
                    new UserRolesByUser()
                    {
                        UserId = userId.ToLower(),
                        RoleId = "SystemUser",
                        OwnedUtcDate = utcNow
                    }
                };

                _context.UserRolesByUsers.AddRange(userRolesByUsers);
                rowAffected += _context.SaveChanges();
            }

            return rowAffected;
        }
    }
}

