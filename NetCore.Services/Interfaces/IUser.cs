using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Interfaces
{
    public interface IUser
    {
        //실제 사용할 서비스메서드 선언
        //MembershipController 의 Login Action메서드에 사용하는 Logininfo View모델을 사용
        //함수를 실질적으로 사용하기 위해 UserService 상속 입력
        bool MatchTheUserInfo(LoginInfo login);

        //UserService.cs에서 가져와서 연결 (14. )
        User GetUserInfo(string userId);

        //UserService.cs 권한부분 가져옴 (14. )
        //이름부분은 수정. GetRolesOwnedbyUser : 사용자 소유의 권한들을 가져오겠다
        IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId);

        /// <summary>
        /// 사용자 가입
        /// </summary>
        /// <param name="register">사용자 가입용 뷰모델</param>
        /// <returns></returns>
        int RegisterUser(RegisterInfo register);

        /// <summary>
        /// [사용자 정보수정을 위한 검색] 18.
        /// </summary>
        /// <param name="userId">사용자 아이디</param>
        /// <returns></returns>
        UserInfo GetUserInfoForUpdate(string userId);

        /// <summary>
        /// [사용자 정보수정] 18.
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        int UpdateUser(UserInfo user);

        /// <summary>
        /// [사용자 정보수정에서 변경대상 비교] true : 전부 똑같을 때, false: 하나라도 다를 때 18.
        /// </summary>
        /// <param name="user">사용자정보 뷰모델</param>
        /// <returns></returns>
        bool CompareInfo(UserInfo user);

        //추가 19.
        /// <summary>
        /// [사용자 탈퇴]
        /// </summary>
        /// <param name="user">사용자탈퇴정보 뷰모델</param>
        /// <returns></returns>
        int WithdrawnUser(WithdrawnInfo user);
    }
}
