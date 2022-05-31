using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    //18.
    public class UserInfo
    {
        //유저아이디
        [Required(ErrorMessage = "사용자 아이디를 입력하세요.")]
        [MinLength(6, ErrorMessage = "사용자 아이디는 6자 이상 입력")]
        [Display(Name = "사용자 아이디")]
        public string UserId { get; set; }        

        //패스워드
        //LoginInfo의 뷰모델의 멤버변수 Password를 Password유형의 데이터 타입으로 변경하겠다. 13.
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀버호를 입력하세요.")]
        [MinLength(6, ErrorMessage = "비밀번호는 6자 이상 입력")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; }

        /************************************수정 부분******************************************/

        //사용자 이름 추가 17.
        [Required(ErrorMessage = "사용자 이름을 입력하세요.")]
        [Display(Name = "사용자 이름")]
        public string UserName { get; set; }

        //사용자 이메일 추가 17.
        [DataType(DataType.EmailAddress)]
        [Display(Name = "사용자 이메일")]
        public string UserEmail { get; set; }

        //ChangeInfo클래스 추가 18.
        public virtual ChangeInfo ChangeInfo { get; set; }
    }
}
