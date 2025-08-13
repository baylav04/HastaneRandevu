using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // 🔹 Eklenmesi şart
using Microsoft.EntityFrameworkCore; // 🔹 Index özelliği için şart
using HastaneRandevu.Validations;

namespace HastaneRandevu.Models
{
    [Index(nameof(TCKimlikNo), IsUnique = true)] // 🔒 Tekil olsun!
    public class Hasta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string AdSoyadi { get; set; }

        [Required(ErrorMessage = "TC Kimlik No zorunludur.")]
        [StringLength(11, ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.")]
        [TCKimlikNoValidation(ErrorMessage = "Geçersiz TC Kimlik Numarası.")]
        public string TCKimlikNo { get; set; }

        [Required(ErrorMessage = "Parola zorunludur.")]
        public string Parola { get; set; }
        public List<Randevu>? Randevular { get; set; }
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string? Email { get; set; }
    }
}

