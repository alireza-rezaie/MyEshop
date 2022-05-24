using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyEshop.Data.Repositories;
using MyEshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyEshop.Controllers
{
    public class AccountController : Controller
    {
        private IUserRepository _UserRepository;
        public AccountController(IUserRepository UserRepository)
        {
            _UserRepository = UserRepository;
        }
        #region Register
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }
            if (_UserRepository.IsExitsUserByEmail(register.Email.ToLower()))
            {
                ModelState.AddModelError("email", "کاربر در وب سایت ثبت نام کرده است");
                return View(register);
            }
            Users user = new Users()
            {
                Email = register.Email.ToLower(),
                Password = register.Password,
                IsAdmin = false,
                RegisterDate = DateTime.Now
            };
            _UserRepository.AddUser(user);
            return View("SuccsessRegister", register);
        }
        //public IActionResult VerifyEmail(string email)
        //{
        //    if (_UserRepository.IsExitsUserByEmail(email.ToLower()))
        //    {
        //        return Json($"ایمیل {email} تکراری است");

        //    }
        //    return Json("True");
        //}
        #endregion
        #region Login
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            var user = _UserRepository.GetUserForLogin(login.Email.ToLower(), login.Password.ToLower());

            if (user==null)
            {
                ModelState.AddModelError("email", "اطلاعات صحیح نیست");
                return View(login);
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString()),
               // new Claim("CodeMeli", user.Email),

            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var properties = new AuthenticationProperties
            {
                IsPersistent = login.RememberMe
            };

            HttpContext.SignInAsync(principal, properties);

            return Redirect("/");
           
        }
        #endregion
        #region Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Account/Login");
        }
        #endregion

    }
}
