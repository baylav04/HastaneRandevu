using System.ComponentModel.DataAnnotations;
namespace HastaneRandevu.Models;
public class ResetPasswordViewModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Token { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    public string NewPassword { get; set; }
}
