using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class WithdrawnInfo
    {
        /// <summary>
        /// 사용자 아이디 / 히든값으로 받음
        /// </summary>
        public string UserId { get; set; }

        //패스워드
        //LoginInfo의 뷰모델의 멤버변수 Password를 Password유형의 데이터 타입으로 변경하겠다. 13.
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀버호를 입력하세요.")]
        [MinLength(6, ErrorMessage = "비밀번호는 6자 이상 입력")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; }
    }
}
