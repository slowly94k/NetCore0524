using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class AESInfo
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

        //암호화는 길어질 수 있으니까 MultilineText 형식으로
        [DataType(DataType.MultilineText)]
        [Display(Name = "암호화 정보")]
        public string EncUserInfo { get; set; }

        [Display(Name = "복호화 정보")]
        public string DecUserInfo { get; set; }
    }
}
