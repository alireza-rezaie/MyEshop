using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyEshop.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300)]
        [EmailAddress]
        [Display(Name ="ایمیل")]
        //[Remote("VerifyEmail", "Account")]

        public string Email { get; set; }

        [Required(ErrorMessage ="لطفا {0} را وارد کنید")]
        [MaxLength(50)]
        //[MinLength(8, ErrorMessage = "کلمه عبور نمیتواند کمتر از 8 کاراکتر باشد")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$", ErrorMessage = "کلمه عبور باید شامل حرف و عدد باشد")]
        public string Password { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "تکرار کلمه عبور")]

        public string RePassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300,ErrorMessage = "نمیتوانید بیشتر از 300 کاراکتر وارد کنید")]
        [EmailAddress]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور")]
        //[MinLength(8, ErrorMessage = "کلمه عبور نمیتواند کمتر از 8 کاراکتر باشد")]
        public string Password { get; set; }

        [Display(Name ="مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
