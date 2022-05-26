using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Controllers
{
    //컨트롤러를 만들때 index액션 메서드가 자동생성 3
    public class MembershipController : Controller
    {
        //닷넷코어는 의존성 주입 패턴을 가진다. 생성자를 통해서 의존성 주입이 이루어진다.
        //전역번수 인터페이스 UserService사용하기 위해 인터페이스(IUser)가 있다
        //의존성 주입 - 생성자 
        private IUser _user;
        private HttpContext _context;


        //생성자 생성
        //파라메터로 IUser인터페이스 추가
        public MembershipController(IHttpContextAccessor accessor, IUser user)
        {
            //전역변수 _user(인터페이스)에 생성자의 파라메터user을 넣어 사용할 수 있게 한다.
            _context = accessor.HttpContext;
            _user = user;
            
        }
        public IActionResult Index()
        {
            return View();//뷰 추가 3
        }
        //Get방식으로 접근했을때 보여지는 view페이지를 위한 액션 메서드 3.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /*아이디, 비번 입력해 로그인 눌리면 이곳으로 온다    3.
        위조방지 토큰을 통해 View로 받은 Post data가 유효한지 검증
        [HttpPost] 방식 Action메서드 일 때 지정 해 줘야한다 	        
        */
        //post로 지정된 Login액션 메서드에서 뷰모델(LoginInfo)을 만들어 줘야한다
        //=> 처음엔 Models폴더 , NetCore.Data솔루션 > ViewModels에서 뷰모델을 관리하기 때문에 폴더로 4.
        //LoginInfo.cs의 두개의 항목을 통해서 post방식의 MembershipController로 들어온다
        //로그인메서드 부분을 비동기로 바꾼다 14.
        //이제 {controller}/Login 컨트롤러까지 지정해줘야한다./ Login으로 넘어오도록 지정 14 
        [HttpPost("/{controller}/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginInfo login)
        {
            //메세지를 여러가지 사용하기 때문에 변수로 선언
            string message = string.Empty;
            //값들이 유효하게 들어 왔는지
            if (ModelState.IsValid)
            {
                //뷰모델
                //서비스 개념
                if (_user.MatchTheUserInfo(login))
                {
                    //신원보증과 승인권한 (실질적으로 여기서 일어난다) 14.
                    //await HttpContext로 사용해도 되지만 의존성주입 방법으로 한다.
                    //userTopRole : 권한이 여러개 일 때 탑 권한 GetRolesOwnedByUser가 정렬이 되어있어야 한다=>구현으로 이동
                    var userInfo = _user.GetUserInfo(login.UserId);//사용자 이름을 가져온다
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault(); 

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                    new Claim(type:ClaimTypes.Name,
                              value:userInfo.UserName),
                    //권한 가져오는 부분
                    new Claim(type:ClaimTypes.Role,
                              value:userTopRole.RoleId + "|" + userTopRole.UserRole.RoleName + "|" + userTopRole.UserRole.RolePriority.ToString())
                    }, authenticationType:CookieAuthenticationDefaults.AuthenticationScheme);

                    //IsPersistent : 지속여부 => LoginInfo.cs에서 지정
                    await _context.SignInAsync(scheme:CookieAuthenticationDefaults.AuthenticationScheme,
                                               principal:new ClaimsPrincipal(identity:identity),
                                               properties:new AuthenticationProperties()
                                               { 
                                                   IsPersistent = login.RememberMe,
                                                   ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                               });


                    //Index.cshtml에 뿌려져야 한다
                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";
                    //페이지 이동 : Index 자동생성된 뷰로, 컨트롤러는 Membership
                    return RedirectToAction("Index", "Membership");
                }
                else
                {
                    message = "로그인되지 않았습니다.";
                }
            }
            else
            {
                message = "로그인정보를 올바르게 입력하세요";
            }
            //else일 때 에러모델 추가
            ModelState.AddModelError(string.Empty, message);
            //View로 View모델 넘길때 어떤 View로 할것인지 정해 줘야함
            return View("Login", login);
        }

        //로그아웃 비동기 메서드(14.)
        [HttpGet("/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br /> 웹사이트를 원활히 하려면 로그인 하세요.";
            //로그아웃이 성공적으로 이루어 지면 Index.cshtml로 간다.
            return RedirectToAction("Index", "Membership");
        }
    }
}