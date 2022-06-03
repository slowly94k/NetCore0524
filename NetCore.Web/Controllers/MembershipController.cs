using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Web.Controllers
{
    //15. 권한부여 
    //컨트롤러를 만들때 index액션 메서드가 자동생성 3
    //사용가능 권한 4개 로그인을 해야 MembershipController로 들어온다 //IActionResult여기로 권한을 해도 된다
    [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
    public class MembershipController : Controller
    {
        //닷넷코어는 의존성 주입 패턴을 가진다. 생성자를 통해서 의존성 주입이 이루어진다.
        //전역번수 인터페이스 UserService사용하기 위해 인터페이스(IUser)가 있다

        //1.전역변수로 인터페이스 설정 (4.의존성 주입)
        //2.의존성 주입 - 생성자(다른 방법도 있다)
        /*
          생성자 주입방식은 생성자의 파라미터를 통해 인터페이스를 지정하여
          서비스클래스 인스턴스를 받아온다.
        */
        private IUser _user;
        private IPasswordHasher _hasher;
        private HttpContext _context;

        //생성자 생성
        //파라메터로 IUser인터페이스 추가
        public MembershipController(IHttpContextAccessor accessor,IPasswordHasher hasher, IUser user)
        {

            _context = accessor.HttpContext;
            _hasher = hasher;
            _user = user;
            //전역변수 _user(인터페이스)에 생성자의 파라메터user을 넣어 사용할 수 있게 한다.
            _context = accessor.HttpContext;
            _user = user;
            

        }

        //15.
        #region private methods
        /// <summary>
        /// 로컬URL인지 외부URL인지 체크
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(MembershipController.Index), "Membership");
            }
        }
        #endregion

        [AllowAnonymous] // 탈퇴 때문에 추가 19.
        public IActionResult Index()
        {
            return View();//뷰 추가 3
        }

        //Get방식으로 접근했을때 보여지는 view페이지를 위한 액션 메서드 3.
        //모든사람에 접근이 되어야한다  [AllowAnonymous]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
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
        [AllowAnonymous]
        //Data => Services => Web 
        //Data => Services
        //Data => Web 웹프로젝트는 데이터프로젝트 참조 (4.의존성)
        public async Task<IActionResult> LoginAsync(LoginInfo login, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //뷰모델
                //데이터베이스를 통해서 데이터모델이 연동되어 값을 가지고 있어서
                //그것과 입력받은 값들과 비교를 해야한다
                //서비스 개념 (수정 4.의존성)
                if (_user.MatchTheUserInfo(login)) //=> 사용안함 16. //다시 사용 17.

                //사용자 정보 아이디와 비밀번호를 받아서 데이터베이스에 있는 정보와 일치하는지 여부를 
                //패스워드 해셔에다가 작성을 함                
                //if (_hasher.MatchTheUserInfo(login.UserId, login.Password)) // 이부분 사용안함 17.
                {

                    //신원보증과 승인권한 (실질적으로 여기서 일어난다) 14.
                    //await HttpContext로 사용해도 되지만 의존성주입 방법으로 한다.
                    //userTopRole : 권한이 여러개 일 때 탑 권한 GetRolesOwnedByUser가 정렬이 되어있어야 한다=>구현으로 이동
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();
                    ////4가지를 UserData에 추가 15.
                    string userDataInfo = userTopRole.UserRole.RoleName + "|" +
                                          userTopRole.UserRole.RolePriority.ToString() + "|" +
                                          userInfo.UserName + "|" +
                                          userInfo.UserEmail;

                    //_context.User.Identity.Name => 사용자 아이디

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                        new Claim(type:ClaimTypes.Name,
                                  value:userInfo.UserId),
                        //권한 가져오는 부분
                        new Claim(type:ClaimTypes.Role,                        
                                  value:userTopRole.RoleId),
                        //ClaimTypes.UserData으로 추가 15.
                        new Claim(type:ClaimTypes.UserData,
                                  value:userDataInfo)
                    //기본쿠키 지정, 계속 반복되는 구문
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    //IsPersistent : 지속여부 => LoginInfo.cs에서 지정
                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                                               principal: new ClaimsPrincipal(identity: identity),
                                               properties: new AuthenticationProperties()
                                               {
                                                   //지속여부 지정
                                                   IsPersistent = login.RememberMe,

                                                   //인증쿠키 만료기간
                                                   ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                               });

                    //Index.cshtml에 뿌려져야 한다
                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";

                    //페이지 이동 : Index 자동생성된 뷰로, 컨트롤러는 Membership
                    //returnUrl의 값이 없을땐 상관 없는데 있으면 그 페이지로 보내줘야한다
                    // =>이렇게 사용 안된다 위에 추가 RedirectToLocal  15.
                    //return RedirectToAction("Index", "Membership"); => 사용안한 15.
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    message = "로그인되지 않습니다.";
                }
            }
            else
            {
                message = "로그인 정보를 올바르게 입력하세요.";
            }
            //else일 때 에러모델 추가
            ModelState.AddModelError(string.Empty, message);
            //View로 View모델 넘길때 어떤 View로 할것인지 정해 줘야함
            return View("Login", login);
        }

        //사용자 가입용 액션메서드 생성 17.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register( string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Register(RegisterInfo register, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //사용자 가입 서비스 17.
                if (_user.RegisterUser(register) > 0)
                {
                    TempData["Message"] = "사용자 가입이 성공적으로 이루어졌습니다.";
                    return RedirectToAction("Login", "Membership");
                }
                else
                {
                    message = "사용자가 가입되지 않았습니다.";
                }
                
            }
            else
            {
                message = "사용자 가입을 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(register);
        }

        //Update메서드 생성 18.
        [HttpGet]
        public IActionResult UpdateInfo()
        {
            UserInfo user = _user.GetUserInfoForUpdate(_context.User.Identity.Name);//서비스 

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(UserInfo user)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //변경대상 값들을 비교 //서비스
                if (_user.CompareInfo(user))
                {
                    message = "하나 이상의 값이 변경되어야 정보수정이 가능합니다.";
                    ModelState.AddModelError(string.Empty, message);
                    return View(user);
                }

                //정보수정 서비스
                if (_user.UpdateUser(user) > 0)
                {
                    TempData["Message"] = "사용자 정보수정이 성공적으로 이루어졌습니다.";

                    return RedirectToAction("UpdateInfo", "Membership");
                }
                else
                {
                    message = "사용자 정보가 수정되지 않았습니다.";
                }
            }
            else
            {
                message = "사용자 정보수정을 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View(user);
        }

        //사용자 탈퇴메서드 19.
        //비동기로 수정
        [HttpPost("/{controller}/Withdrawn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawnAsync(WithdrawnInfo withdrawn)
        {
            string message = string.Empty;
            //모델상태 체크            
            if (ModelState.IsValid)
            {
                //탈퇴 서비스
                if (_user.WithdrawnUser(withdrawn) > 0)
                {
                    TempData["Message"] = "사용자 탈퇴가 성공적으로 이루어졌습니다.";

                    //비동기니까 await로~
                    await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

                    return RedirectToAction("Index", "Membership");
                }
                else
                {
                    message = "사용자가 탈퇴처리되지 않았습니다.";
                }

            }
            else
            {
                message = "사용자가 탈퇴하기 위한 정보를 올바르게 입력하세요.";
            }

            ViewData["Message"] = message;
            return View("Index", withdrawn);
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

        //접근제한 페이지 15.
        /*
         팅겨져 나오면 Forbidden()여기로 오는데 Forbidden()의 입장에서 이전에 요청했던 URL은 
         팅겨나간 그 페이지! 그 페이지를 화면에 표시하기 위해서 returnUrl을 사용
         */
        //[FromServices] : Action메서드에서 파라미터로 의존성 주입
        [HttpGet]
        //[Authorize(Roles = "AssociateUser")] 16. 시작부분에서 주석처리 됨
        public IActionResult Forbidden([FromServices]ILogger<MembershipController> logger)
        {
            //StringValues paramReturnUrl; 없어짐 16.  paramReturnUrl[0] =? /Data/AES
            //존재여부  returnUrl에 대해서 paramReturnUrl이 있는지 체크
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out StringValues paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            //MethodBase.GetCurrentMethod().Name : 현재의 메서드의 이름을 가져온다.
            logger.LogTrace($"{MethodBase.GetCurrentMethod().Name} 메서드.권한이 없는 사람이 페이지에 접근 에러 처리.returnUrl : {paramReturnUrl}");

            ViewData["Message"] = $"귀하는 {paramReturnUrl} 경로로 접근하려고 했습니다만, <br>" +
                                   "인증된 사용자도 접근하지 못하는 페이지가 있습니다. <br>" +
                                   "담당자에게 해당페이지의 접근권한에 대해 문의하세요.";
            return View();
        }
    }
}
