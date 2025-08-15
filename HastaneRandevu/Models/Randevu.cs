using System.ComponentModel.DataAnnotations;

namespace HastaneRandevu.Models
{
    public class Randevu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Poliklinik seçimi zorunludur.")]
        public int PoliklinikId { get; set; }
        public Poliklinik? Poliklinik { get; set; }

        [Required(ErrorMessage = "Doktor seçimi zorunludur.")]
        public int DoktorId { get; set; }
        public Doktor? Doktor { get; set; }

        [Required(ErrorMessage = "Hasta seçimi zorunludur.")]
        public int HastaId { get; set; }
        public Hasta? Hasta { get; set; }

        [Required(ErrorMessage = "Randevu saati seçilmelidir.")]
        [DataType(DataType.DateTime)]
        public DateTime RandevuSaati { get; set; }

        [Required]
        public string Durum { get; set; } = "aktif";

        // ✅ Yeni Eklenen Alanlar
        public bool SmsGonderildi { get; set; } = false;
        public bool SmsIletildi { get; set; } = false;
    }
}
