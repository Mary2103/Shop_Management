using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SV20T1020285.BusinessLayers;
using SV20T1020285.DomainModels;
using System.Reflection;
using System.Security.Claims;

namespace SV20T1020285.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.Username = username;

            //Kiểm tra xem thông tin nhập có đủ không (đầu vào)
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đủ tên và mật khẩu");
                return View();
            }

            //Kiểm tra thông tin đăng nhập có hợp lệ hay không
            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Đăng nhập thành công, tạo dữ liệu để lưu cookie
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //Thiết lập phiên đăng nhập cho tài khoản
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string userName = "", string oldPassword = "", string newPassword = "", string repeatPassword = "")
        {
            ViewBag.oldPassword = oldPassword;
            
            if (string.IsNullOrWhiteSpace(oldPassword))
                ModelState.AddModelError("oldPassword", "Bạn phải nhập mật khẩu hiện tại");

            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Bạn phải nhập mật khẩu mới");

            if (string.IsNullOrWhiteSpace(repeatPassword))
                ModelState.AddModelError("repeatPassword", "Bạn phải xác nhận mật khẩu mới");


            // Kiểm tra xem mật khẩu mới và mật khẩu xác nhận có khớp nhau không
            if (string.IsNullOrWhiteSpace(oldPassword) && string.IsNullOrWhiteSpace(newPassword) && newPassword != repeatPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và mật khẩu xác nhận không khớp nhau.";
                return View();
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool isChangePassword = UserAccountService.ChangePassword(userName, oldPassword, newPassword);
            if (!isChangePassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu hiện tại không chính xác";
                return View();

            }
            else
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công. Bạn cần đăng nhập lại !";
                return View();
            }

        }

        public async Task<IActionResult> AccessDenined()
        {
            return View();
        }


    }   
}
