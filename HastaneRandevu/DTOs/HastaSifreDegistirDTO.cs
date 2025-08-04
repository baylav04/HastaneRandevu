using System.ComponentModel.DataAnnotations;

namespace HastaneRandevu.DTOs
{
    public class HastaSifreDegistirDTO
    {        
        [Required(ErrorMessage = "TC Kimlik No alanı zorunludur")]
        [MinLength(11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        [MaxLength(11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string TCKimlikNo { get; set; }

        [Required(ErrorMessage = "Yeni şifre alanı zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifre tekrar alanı zorunludur")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; }
    }
}
