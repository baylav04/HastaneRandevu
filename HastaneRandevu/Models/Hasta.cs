using System.ComponentModel.DataAnnotations;
using HastaneRandevu.Validations; // Özel validasyon sınıfını çağırmak için

namespace HastaneRandevu.Models
{
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
    }
}
