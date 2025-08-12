using HastaneRandevu.Areas.Identity.Data;
using HastaneRandevu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HastaneRandevu.Models;

namespace HastaneRandevu.Controllers
{
    [Authorize(Roles = "Admin")]  // Sadece Admin erişebilir
    public class AccountController : Controller
    {
        private readonly UserManager<HastaneRandevuUser> _userManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<HastaneRandevuUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Lütfen e-posta adresinizi girin.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Güvenlik için kullanıcı yoksa da aynı mesajı döner
                return View("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            var resetLink = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = encodedToken }, Request.Scheme);

            var message = $"Şifreni sıfırlamak için <a href='{resetLink}'>buraya tıklayın</a>.";

            await _emailService.SendEmailAsync(email, "Şifre Sıfırlama", message);

            return View("ForgotPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Geçersiz şifre sıfırlama isteği.");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(model.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}

