using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HastaneRandevu.Filters
{
    public class HastaAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Oturum açan hastanýn ID'si session'da mevcut mu kontrol et
            if (!context.HttpContext.Session.Keys.Contains("LoggedInHastaId"))
            {
                // Eðer oturum açýlmamýþsa, login sayfasýna yönlendir
                context.Result = new RedirectToActionResult("Login", "Hastas", null);
            }
        }
    }

    // Kolay kullaným için Action ve Controller seviyesinde kullanýlabilecek öznitelik
    public class HastaAuthorizeAttribute : TypeFilterAttribute
    {
        public HastaAuthorizeAttribute() : base(typeof(HastaAuthorizationFilter))
        {
        }
    }
}