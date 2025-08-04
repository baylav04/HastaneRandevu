using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HastaneRandevu.Filters
{
    public class HastaAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Oturum a�an hastan�n ID'si session'da mevcut mu kontrol et
            if (!context.HttpContext.Session.Keys.Contains("LoggedInHastaId"))
            {
                // E�er oturum a��lmam��sa, login sayfas�na y�nlendir
                context.Result = new RedirectToActionResult("Login", "Hastas", null);
            }
        }
    }

    // Kolay kullan�m i�in Action ve Controller seviyesinde kullan�labilecek �znitelik
    public class HastaAuthorizeAttribute : TypeFilterAttribute
    {
        public HastaAuthorizeAttribute() : base(typeof(HastaAuthorizationFilter))
        {
        }
    }
}